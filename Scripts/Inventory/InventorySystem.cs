using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string description;
    public Sprite icon;
    public ItemType itemType;
    public int quantity = 1;
    public int value;
    
    // Equipment stats
    public int healthBonus;
    public int damageBonus;
    public int defenseBonus;
    
    public InventoryItem(string name, string desc, Sprite icon, ItemType type, int val = 0)
    {
        itemName = name;
        description = desc;
        this.icon = icon;
        itemType = type;
        value = val;
    }
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Material,
    Quest
}

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxInventorySlots = 20;
    public List<InventoryItem> items = new List<InventoryItem>();
    
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform itemContainer;
    public GameObject itemSlotPrefab;
    public Text goldText;
    
    [Header("Equipment")]
    public InventoryItem equippedWeapon;
    public InventoryItem equippedArmor;
    
    // Private variables
    private bool isInventoryOpen = false;
    private int gold = 0;
    
    void Start()
    {
        // Initialize with some basic items
        AddItem(new InventoryItem("Health Potion", "Restores 25 health", null, ItemType.Consumable, 10));
        AddItem(new InventoryItem("Basic Sword", "A simple sword", null, ItemType.Weapon, 50) { damageBonus = 10 });
        AddItem(new InventoryItem("Leather Armor", "Basic protection", null, ItemType.Armor, 30) { defenseBonus = 5 });
        
        UpdateInventoryUI();
        UpdateGoldUI();
    }
    
    void Update()
    {
        // Toggle inventory with I key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
    
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        
        if (isInventoryOpen)
        {
            UpdateInventoryUI();
        }
    }
    
    public bool AddItem(InventoryItem item)
    {
        if (items.Count >= maxInventorySlots)
        {
            Debug.Log("Inventory is full!");
            return false;
        }
        
        // Check if item already exists and is stackable
        if (item.itemType == ItemType.Consumable || item.itemType == ItemType.Material)
        {
            InventoryItem existingItem = items.Find(x => x.itemName == item.itemName);
            if (existingItem != null)
            {
                existingItem.quantity += item.quantity;
                UpdateInventoryUI();
                return true;
            }
        }
        
        items.Add(item);
        UpdateInventoryUI();
        return true;
    }
    
    public void RemoveItem(InventoryItem item)
    {
        items.Remove(item);
        UpdateInventoryUI();
    }
    
    public void UseItem(InventoryItem item)
    {
        switch (item.itemType)
        {
            case ItemType.Consumable:
                UseConsumable(item);
                break;
            case ItemType.Weapon:
                EquipWeapon(item);
                break;
            case ItemType.Armor:
                EquipArmor(item);
                break;
        }
    }
    
    void UseConsumable(InventoryItem item)
    {
        if (item.itemName.Contains("Health Potion"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.Heal(25);
                item.quantity--;
                
                if (item.quantity <= 0)
                {
                    RemoveItem(item);
                }
                else
                {
                    UpdateInventoryUI();
                }
            }
        }
    }
    
    void EquipWeapon(InventoryItem weapon)
    {
        // Unequip current weapon
        if (equippedWeapon != null)
        {
            // Remove weapon bonuses from player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.attackDamage -= equippedWeapon.damageBonus;
            }
        }
        
        equippedWeapon = weapon;
        
        // Apply weapon bonuses to player
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.attackDamage += weapon.damageBonus;
        }
        
        UpdateInventoryUI();
    }
    
    void EquipArmor(InventoryItem armor)
    {
        // Unequip current armor
        if (equippedArmor != null)
        {
            // Remove armor bonuses from player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.maxHealth -= equippedArmor.healthBonus;
                player.currentHealth = Mathf.Min(player.currentHealth, player.maxHealth);
            }
        }
        
        equippedArmor = armor;
        
        // Apply armor bonuses to player
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.maxHealth += armor.healthBonus;
            playerController.currentHealth += armor.healthBonus;
        }
        
        UpdateInventoryUI();
    }
    
    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
    }
    
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldUI();
            return true;
        }
        return false;
    }
    
    void UpdateInventoryUI()
    {
        if (!isInventoryOpen) return;
        
        // Clear existing UI
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create item slots
        for (int i = 0; i < items.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();
            
            if (slotScript != null)
            {
                slotScript.SetupSlot(items[i], this);
            }
        }
    }
    
    void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {gold}";
        }
    }
}
