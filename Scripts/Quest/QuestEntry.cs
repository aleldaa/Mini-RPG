using UnityEngine;
using UnityEngine.UI;

public class QuestEntry : MonoBehaviour
{
    [Header("UI Elements")]
    public Text questNameText;
    public Text descriptionText;
    public Text progressText;
    public Button acceptButton;
    public Button completeButton;
    
    private Quest quest;
    private QuestSystem questSystem;
    
    public void SetupQuest(Quest newQuest, QuestSystem system)
    {
        quest = newQuest;
        questSystem = system;
        
        if (quest != null)
        {
            if (questNameText != null)
            {
                questNameText.text = quest.questName;
            }
            
            if (descriptionText != null)
            {
                descriptionText.text = quest.description;
            }
            
            if (progressText != null)
            {
                progressText.text = $"Progress: {quest.currentAmount}/{quest.targetAmount}";
            }
            
            // Setup buttons
            if (acceptButton != null)
            {
                acceptButton.gameObject.SetActive(!quest.isActive);
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(AcceptQuest);
            }
            
            if (completeButton != null)
            {
                completeButton.gameObject.SetActive(quest.isCompleted);
                completeButton.onClick.RemoveAllListeners();
                completeButton.onClick.AddListener(CompleteQuest);
            }
        }
    }
    
    public void AcceptQuest()
    {
        if (questSystem != null && quest != null)
        {
            questSystem.AcceptQuest(quest);
        }
    }
    
    public void CompleteQuest()
    {
        if (questSystem != null && quest != null)
        {
            questSystem.CompleteQuest(quest);
        }
    }
    
    public void UpdateProgress()
    {
        if (quest != null && progressText != null)
        {
            progressText.text = $"Progress: {quest.currentAmount}/{quest.targetAmount}";
            
            // Update button states
            if (acceptButton != null)
            {
                acceptButton.gameObject.SetActive(!quest.isActive);
            }
            
            if (completeButton != null)
            {
                completeButton.gameObject.SetActive(quest.isCompleted);
            }
        }
    }
}
