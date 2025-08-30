using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    public Image itemIcon;
    public Text itemNameText;
    public Text quantityText;
    public Text descriptionText;
    
    private InventoryItem item;
    private InventorySystem inventorySystem;
    
    public void SetupSlot(InventoryItem newItem, InventorySystem system)
    {
        item = newItem;
        inventorySystem = system;
        
        if (item != null)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true;
            }
            
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName;
            }
            
            if (quantityText != null)
            {
                quantityText.text = item.quantity > 1 ? $"x{item.quantity}" : "";
            }
            
            if (descriptionText != null)
            {
                descriptionText.text = item.description;
            }
        }
        else
        {
            ClearSlot();
        }
    }
    
    public void ClearSlot()
    {
        item = null;
        
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
        
        if (itemNameText != null)
        {
            itemNameText.text = "";
        }
        
        if (quantityText != null)
        {
            quantityText.text = "";
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Left click - use item
            UseItem();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Right click - show item info or drop item
            ShowItemInfo();
        }
    }
    
    void UseItem()
    {
        if (inventorySystem != null && item != null)
        {
            inventorySystem.UseItem(item);
        }
    }
    
    void ShowItemInfo()
    {
        if (item != null)
        {
            string info = $"Name: {item.itemName}\n";
            info += $"Type: {item.itemType}\n";
            info += $"Description: {item.description}\n";
            info += $"Value: {item.value} gold\n";
            
            if (item.damageBonus > 0)
                info += $"Damage Bonus: +{item.damageBonus}\n";
            if (item.healthBonus > 0)
                info += $"Health Bonus: +{item.healthBonus}\n";
            if (item.defenseBonus > 0)
                info += $"Defense Bonus: +{item.defenseBonus}\n";
            
            Debug.Log(info);
        }
    }
}
