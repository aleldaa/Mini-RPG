using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [Header("Turn Display")]
    public Text turnText;
    public Text currentCombatantText;
    public Image turnIndicator;
    
    [Header("Action Buttons")]
    public Button moveButton;
    public Button attackButton;
    public Button endTurnButton;
    public Button itemButton;
    
    [Header("Combat Status")]
    public GameObject combatPanel;
    public Text combatStatusText;
    public Slider playerHealthBar;
    public Text playerHealthText;
    
    [Header("References")]
    public TurnManager turnManager;
    public Combatant currentCombatant;
    
    void Start()
    {
        if (turnManager == null)
            turnManager = FindObjectOfType<TurnManager>();
        
        // Subscribe to events
        if (turnManager != null)
        {
            turnManager.OnCombatStarted += OnCombatStarted;
            turnManager.OnCombatEnded += OnCombatEnded;
            turnManager.OnTurnChanged += OnTurnChanged;
        }
        
        // Set up button listeners
        SetupButtons();
        
        // Initially hide combat UI
        if (combatPanel != null)
            combatPanel.SetActive(false);
    }
    
    void SetupButtons()
    {
        if (moveButton != null)
            moveButton.onClick.AddListener(OnMoveButtonClicked);
        
        if (attackButton != null)
            attackButton.onClick.AddListener(OnAttackButtonClicked);
        
        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
        
        if (itemButton != null)
            itemButton.onClick.AddListener(OnItemButtonClicked);
    }
    
    public void UpdateTurnDisplay(Combatant combatant)
    {
        currentCombatant = combatant;
        
        if (currentCombatantText != null)
        {
            currentCombatantText.text = combatant.isPlayer ? "Your Turn" : $"{combatant.name}'s Turn";
        }
        
        if (turnText != null)
        {
            turnText.text = $"Turn {turnManager.currentTurnIndex + 1}";
        }
        
        // Update action buttons based on what the combatant can do
        UpdateActionButtons();
        
        // Update health display if it's the player
        if (combatant.isPlayer)
        {
            UpdatePlayerHealth();
        }
    }
    
    void UpdateActionButtons()
    {
        if (currentCombatant == null) return;
        
        bool isPlayerTurn = currentCombatant.isPlayer;
        
        // Show/hide buttons based on turn
        if (combatPanel != null)
            combatPanel.SetActive(isPlayerTurn);
        
        if (moveButton != null)
        {
            moveButton.interactable = currentCombatant.CanMove();
            moveButton.GetComponentInChildren<Text>().text = "Move";
        }
        
        if (attackButton != null)
        {
            attackButton.interactable = currentCombatant.CanAttack();
            attackButton.GetComponentInChildren<Text>().text = "Attack";
        }
        
        if (endTurnButton != null)
        {
            endTurnButton.interactable = isPlayerTurn;
        }
    }
    
    void UpdatePlayerHealth()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.value = (float)currentCombatant.currentHealth / currentCombatant.maxHealth;
        }
        
        if (playerHealthText != null)
        {
            playerHealthText.text = $"HP: {currentCombatant.currentHealth}/{currentCombatant.maxHealth}";
        }
    }
    
    // Button event handlers
    void OnMoveButtonClicked()
    {
        if (currentCombatant != null && currentCombatant.CanMove())
        {
            // Enable movement (GridMovement will handle the rest)
            GridMovement gridMovement = currentCombatant.GetComponent<GridMovement>();
            if (gridMovement != null)
            {
                gridMovement.EnableMovement();
            }
            
            // Disable move button
            if (moveButton != null)
                moveButton.interactable = false;
        }
    }
    
    void OnAttackButtonClicked()
    {
        if (currentCombatant != null && currentCombatant.CanAttack())
        {
            // Show attack range and enable attack selection
            ShowAttackRange();
            
            // Disable attack button
            if (attackButton != null)
                attackButton.interactable = false;
        }
    }
    
    void OnEndTurnButtonClicked()
    {
        if (currentCombatant != null && currentCombatant.isPlayer)
        {
            // Force end turn
            currentCombatant.EndTurn();
            if (turnManager != null)
            {
                turnManager.OnPlayerActionCompleted();
            }
        }
    }
    
    void OnItemButtonClicked()
    {
        // Open inventory/use item
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory != null)
        {
            inventory.ToggleInventory();
        }
    }
    
    void ShowAttackRange()
    {
        // This would show the attack range visually
        // For now, we'll just log it
        Debug.Log($"Showing attack range for {currentCombatant.name}");
        
        // You could implement visual indicators here
        // Like highlighting valid attack targets
    }
    
    // Event handlers
    void OnCombatStarted()
    {
        if (combatPanel != null)
            combatPanel.SetActive(true);
        
        if (combatStatusText != null)
            combatStatusText.text = "Combat Started!";
    }
    
    void OnCombatEnded()
    {
        if (combatPanel != null)
            combatPanel.SetActive(false);
        
        if (combatStatusText != null)
            combatStatusText.text = "Combat Ended";
    }
    
    void OnTurnChanged()
    {
        // Turn changed, update UI
        if (turnManager != null && turnManager.isInCombat)
        {
            UpdateTurnDisplay(turnManager.combatants[turnManager.currentTurnIndex]);
        }
    }
    
    // Public methods for external use
    public void ShowCombatMessage(string message)
    {
        if (combatStatusText != null)
        {
            combatStatusText.text = message;
        }
    }
    
    public void UpdateCombatStatus(string status)
    {
        if (combatStatusText != null)
        {
            combatStatusText.text = status;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (turnManager != null)
        {
            turnManager.OnCombatStarted -= OnCombatStarted;
            turnManager.OnCombatEnded -= OnCombatEnded;
            turnManager.OnTurnChanged -= OnTurnChanged;
        }
    }
}
