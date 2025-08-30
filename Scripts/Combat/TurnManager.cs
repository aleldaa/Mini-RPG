using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    public float turnDelay = 1f;
    public bool isPlayerTurn = true;
    
    [Header("Combat State")]
    public bool isInCombat = false;
    public List<Combatant> combatants = new List<Combatant>();
    public int currentTurnIndex = 0;
    
    [Header("References")]
    public CombatUI combatUI;
    public GridMovement playerMovement;
    
    // Events
    public System.Action OnTurnChanged;
    public System.Action OnCombatStarted;
    public System.Action OnCombatEnded;
    
    void Start()
    {
        // Find all combatants in the scene
        FindCombatants();
    }
    
    void FindCombatants()
    {
        combatants.Clear();
        
        // Add player
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            Combatant playerCombatant = player.GetComponent<Combatant>();
            if (playerCombatant == null)
            {
                playerCombatant = player.gameObject.AddComponent<Combatant>();
            }
            combatants.Add(playerCombatant);
        }
        
        // Add enemies
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Combatant enemyCombatant = enemy.GetComponent<Combatant>();
            if (enemyCombatant == null)
            {
                enemyCombatant = enemy.gameObject.AddComponent<Combatant>();
            }
            combatants.Add(enemyCombatant);
        }
    }
    
    public void StartCombat()
    {
        if (isInCombat) return;
        
        isInCombat = true;
        isPlayerTurn = true;
        currentTurnIndex = 0;
        
        // Sort combatants by speed (highest goes first)
        combatants.Sort((a, b) => b.speed.CompareTo(a.speed));
        
        // Initialize combat state for all combatants
        foreach (Combatant combatant in combatants)
        {
            combatant.InitializeCombat();
        }
        
        // Start first turn
        StartTurn();
        
        OnCombatStarted?.Invoke();
    }
    
    public void EndCombat()
    {
        if (!isInCombat) return;
        
        isInCombat = false;
        
        // Reset combat state for all combatants
        foreach (Combatant combatant in combatants)
        {
            combatant.EndCombat();
        }
        
        OnCombatEnded?.Invoke();
    }
    
    public void StartTurn()
    {
        if (!isInCombat) return;
        
        Combatant currentCombatant = combatants[currentTurnIndex];
        
        if (currentCombatant == null || currentCombatant.isDead)
        {
            NextTurn();
            return;
        }
        
        // Check if combat should end
        if (CheckCombatEnd())
        {
            EndCombat();
            return;
        }
        
        // Start the current combatant's turn
        currentCombatant.StartTurn();
        
        // Update UI
        if (combatUI != null)
        {
            combatUI.UpdateTurnDisplay(currentCombatant);
        }
        
        // Handle AI turns
        if (currentCombatant.isPlayer)
        {
            // Player turn - wait for input
            if (playerMovement != null)
            {
                playerMovement.EnableMovement();
            }
        }
        else
        {
            // Enemy turn - AI will handle it
            StartCoroutine(EnemyTurn(currentCombatant));
        }
    }
    
    IEnumerator EnemyTurn(Combatant enemy)
    {
        yield return new WaitForSeconds(turnDelay);
        
        // Let the enemy AI make its move
        enemy.ExecuteAITurn();
        
        // End turn after AI action
        yield return new WaitForSeconds(turnDelay);
        NextTurn();
    }
    
    public void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % combatants.Count;
        
        // Skip dead combatants
        while (combatants[currentTurnIndex].isDead && currentTurnIndex != 0)
        {
            currentTurnIndex = (currentTurnIndex + 1) % combatants.Count;
        }
        
        // If we've gone through all combatants, start over
        if (currentTurnIndex == 0 && combatants[0].isDead)
        {
            EndCombat();
            return;
        }
        
        OnTurnChanged?.Invoke();
        StartTurn();
    }
    
    bool CheckCombatEnd()
    {
        bool playerAlive = false;
        bool enemyAlive = false;
        
        foreach (Combatant combatant in combatants)
        {
            if (combatant.isDead) continue;
            
            if (combatant.isPlayer)
                playerAlive = true;
            else
                enemyAlive = true;
        }
        
        // Combat ends if all players or all enemies are dead
        return !playerAlive || !enemyAlive;
    }
    
    public void OnCombatantDied(Combatant deadCombatant)
    {
        // Remove from combat list
        combatants.Remove(deadCombatant);
        
        // Check if combat should end
        if (CheckCombatEnd())
        {
            EndCombat();
        }
        else if (currentTurnIndex >= combatants.Count)
        {
            // Adjust turn index if needed
            currentTurnIndex = 0;
        }
    }
    
    public void OnPlayerActionCompleted()
    {
        // Player has completed their action, move to next turn
        if (isInCombat && isPlayerTurn)
        {
            NextTurn();
        }
    }
}
