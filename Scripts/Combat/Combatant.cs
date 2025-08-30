using UnityEngine;
using System.Collections;

public class Combatant : MonoBehaviour
{
    [Header("Combat Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 20;
    public int defense = 5;
    public int speed = 10;
    public int moveRange = 3;
    public int attackRange = 1;
    
    [Header("Combat State")]
    public bool isPlayer = false;
    public bool isDead = false;
    public bool hasMoved = false;
    public bool hasAttacked = false;
    public bool isMyTurn = false;
    
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    // Private variables
    private Vector3 originalPosition;
    private TurnManager turnManager;
    private GridMovement gridMovement;
    private Enemy enemyAI;
    
    void Start()
    {
        currentHealth = maxHealth;
        originalPosition = transform.position;
        
        // Get references
        turnManager = FindObjectOfType<TurnManager>();
        gridMovement = GetComponent<GridMovement>();
        enemyAI = GetComponent<Enemy>();
        
        // Determine if this is the player
        isPlayer = GetComponent<PlayerController>() != null;
        
        // Subscribe to events
        if (turnManager != null)
        {
            turnManager.OnCombatStarted += OnCombatStarted;
            turnManager.OnCombatEnded += OnCombatEnded;
        }
    }
    
    public void InitializeCombat()
    {
        hasMoved = false;
        hasAttacked = false;
        isMyTurn = false;
    }
    
    public void EndCombat()
    {
        hasMoved = false;
        hasAttacked = false;
        isMyTurn = false;
        
        // Return to original position if not dead
        if (!isDead)
        {
            transform.position = originalPosition;
        }
    }
    
    public void StartTurn()
    {
        isMyTurn = true;
        hasMoved = false;
        hasAttacked = false;
        
        // Highlight the current combatant
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }
        
        if (isPlayer)
        {
            // Enable player input
            if (gridMovement != null)
            {
                gridMovement.EnableMovement();
            }
        }
    }
    
    public void EndTurn()
    {
        isMyTurn = false;
        
        // Remove highlight
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        // Disable player input
        if (gridMovement != null)
        {
            gridMovement.DisableMovement();
        }
    }
    
    public bool CanMove()
    {
        return isMyTurn && !hasMoved && !isDead;
    }
    
    public bool CanAttack()
    {
        return isMyTurn && !hasAttacked && !isDead;
    }
    
    public void OnMoveCompleted()
    {
        hasMoved = true;
        
        // Check if turn should end
        if (hasMoved && hasAttacked)
        {
            EndTurn();
            if (turnManager != null)
            {
                turnManager.OnPlayerActionCompleted();
            }
        }
    }
    
    public void OnAttackCompleted()
    {
        hasAttacked = true;
        
        // Check if turn should end
        if (hasMoved && hasAttacked)
        {
            EndTurn();
            if (turnManager != null)
            {
                turnManager.OnPlayerActionCompleted();
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        // Apply defense
        int actualDamage = Mathf.Max(1, damage - defense);
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        // Play hit animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        
        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // Remove from combat
        if (turnManager != null)
        {
            turnManager.OnCombatantDied(this);
        }
        
        // Disable components
        if (gridMovement != null)
        {
            gridMovement.enabled = false;
        }
        
        // Destroy after animation
        StartCoroutine(DestroyAfterAnimation());
    }
    
    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    
    // AI Methods (for enemies)
    public void ExecuteAITurn()
    {
        if (isPlayer || isDead) return;
        
        StartCoroutine(AITurnSequence());
    }
    
    IEnumerator AITurnSequence()
    {
        // Simple AI: Move towards player if possible, then attack if in range
        
        // Check if we can move
        if (CanMove())
        {
            // Try to move towards player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                Vector3 targetPos = transform.position + direction * moveRange;
                
                // Move to target position
                transform.position = targetPos;
                hasMoved = true;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Check if we can attack
        if (CanAttack())
        {
            // Look for targets in attack range
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (Collider2D target in targets)
            {
                Combatant targetCombatant = target.GetComponent<Combatant>();
                if (targetCombatant != null && targetCombatant.isPlayer && !targetCombatant.isDead)
                {
                    // Attack the player
                    Attack(targetCombatant);
                    break;
                }
            }
        }
        
        // End turn
        EndTurn();
        if (turnManager != null)
        {
            turnManager.NextTurn();
        }
    }
    
    void Attack(Combatant target)
    {
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Deal damage
        target.TakeDamage(attackDamage);
        hasAttacked = true;
    }
    
    void OnCombatStarted()
    {
        InitializeCombat();
    }
    
    void OnCombatEnded()
    {
        EndCombat();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (turnManager != null)
        {
            turnManager.OnCombatStarted -= OnCombatStarted;
            turnManager.OnCombatEnded -= OnCombatEnded;
        }
    }
}
