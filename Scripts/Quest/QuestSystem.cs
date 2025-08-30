using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Quest
{
    public string questName;
    public string description;
    public QuestType questType;
    public int targetAmount;
    public int currentAmount;
    public bool isCompleted;
    public bool isActive;
    
    // Rewards
    public int experienceReward;
    public int goldReward;
    public List<InventoryItem> itemRewards;
    
    public Quest(string name, string desc, QuestType type, int target, int expReward, int goldReward)
    {
        questName = name;
        description = desc;
        questType = type;
        targetAmount = target;
        currentAmount = 0;
        isCompleted = false;
        isActive = false;
        experienceReward = expReward;
        goldReward = goldReward;
        itemRewards = new List<InventoryItem>();
    }
}

public enum QuestType
{
    KillEnemies,
    CollectItems,
    ReachLocation,
    TalkToNPC
}

public class QuestSystem : MonoBehaviour
{
    [Header("Quest Settings")]
    public List<Quest> availableQuests = new List<Quest>();
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();
    
    [Header("UI References")]
    public GameObject questPanel;
    public Transform questContainer;
    public GameObject questEntryPrefab;
    public Text questCountText;
    
    // Private variables
    private bool isQuestPanelOpen = false;
    
    void Start()
    {
        InitializeQuests();
    }
    
    void Update()
    {
        // Toggle quest panel with Q key
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleQuestPanel();
        }
    }
    
    void InitializeQuests()
    {
        // Create some sample quests
        Quest killQuest = new Quest("Slay the Beasts", "Defeat 5 enemies", QuestType.KillEnemies, 5, 100, 50);
        Quest collectQuest = new Quest("Gather Materials", "Collect 3 health potions", QuestType.CollectItems, 3, 75, 25);
        
        availableQuests.Add(killQuest);
        availableQuests.Add(collectQuest);
        
        UpdateQuestUI();
    }
    
    public void AcceptQuest(Quest quest)
    {
        if (availableQuests.Contains(quest) && !quest.isActive)
        {
            quest.isActive = true;
            availableQuests.Remove(quest);
            activeQuests.Add(quest);
            
            UpdateQuestUI();
        }
    }
    
    public void CompleteQuest(Quest quest)
    {
        if (activeQuests.Contains(quest) && quest.isCompleted)
        {
            // Give rewards
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.GainExperience(quest.experienceReward);
            }
            
            InventorySystem inventory = FindObjectOfType<InventorySystem>();
            if (inventory != null)
            {
                inventory.AddGold(quest.goldReward);
                
                foreach (InventoryItem item in quest.itemRewards)
                {
                    inventory.AddItem(item);
                }
            }
            
            // Move to completed quests
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            
            UpdateQuestUI();
        }
    }
    
    public void UpdateQuestProgress(QuestType type, int amount = 1)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questType == type && !quest.isCompleted)
            {
                quest.currentAmount += amount;
                
                if (quest.currentAmount >= quest.targetAmount)
                {
                    quest.isCompleted = true;
                    Debug.Log($"Quest '{quest.questName}' completed!");
                }
                
                UpdateQuestUI();
            }
        }
    }
    
    public void ToggleQuestPanel()
    {
        isQuestPanelOpen = !isQuestPanelOpen;
        questPanel.SetActive(isQuestPanelOpen);
        
        if (isQuestPanelOpen)
        {
            UpdateQuestUI();
        }
    }
    
    void UpdateQuestUI()
    {
        if (!isQuestPanelOpen) return;
        
        // Clear existing UI
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Show active quests
        foreach (Quest quest in activeQuests)
        {
            GameObject questEntry = Instantiate(questEntryPrefab, questContainer);
            QuestEntry entryScript = questEntry.GetComponent<QuestEntry>();
            
            if (entryScript != null)
            {
                entryScript.SetupQuest(quest, this);
            }
        }
        
        // Update quest count
        if (questCountText != null)
        {
            questCountText.text = $"Active Quests: {activeQuests.Count}";
        }
    }
    
    // Helper methods for different quest types
    public void OnEnemyKilled()
    {
        UpdateQuestProgress(QuestType.KillEnemies);
    }
    
    public void OnItemCollected(string itemName)
    {
        // Check if any active quests require this item
        foreach (Quest quest in activeQuests)
        {
            if (quest.questType == QuestType.CollectItems && quest.description.Contains(itemName))
            {
                UpdateQuestProgress(QuestType.CollectItems);
                break;
            }
        }
    }
}
