using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    public int currentHealth;
    public int damage = 15;
    public float moveSpeed = 2f;
    public int experienceReward = 25;
    public int goldReward = 10;
    
    [Header("AI")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public LayerMask playerLayer;
    
    [Header("UI")]
    public Slider healthBar;
    
    // Private variables
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;
    
    // Turn-based combat components
    private Combatant combatant;
    private bool isInCombat = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Get turn-based component
        combatant = GetComponent<Combatant>();
        
        UpdateHealthBar();
    }
    
    void Update()
    {
        if (isDead) return;
        
        if (isInCombat)
        {
            // In combat, behavior is handled by Combatant component
            HandleCombatState();
        }
        else
        {
            // Exploration mode - simple patrol or idle behavior
            HandleExplorationState();
        }
    }
    
    void HandleCombatState()
    {
        // Combat behavior is handled by the Combatant component
        // This method can be used for additional combat-specific logic
        
        // Update health bar
        UpdateHealthBar();
    }
    
    void HandleExplorationState()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            // Player detected - could start combat or just alert
            // For now, we'll just log it
            Debug.Log($"Player detected by {gameObject.name}");
        }
    }
    
    public void OnCombatStarted()
    {
        isInCombat = true;
        
        // Stop any movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        Debug.Log($"{gameObject.name} entered combat mode");
    }
    
    public void OnCombatEnded()
    {
        isInCombat = false;
        
        // Return to exploration mode
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        Debug.Log($"{gameObject.name} returned to exploration mode");
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        UpdateHealthBar();
        
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
        
        // Give rewards to player
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.GainExperience(experienceReward);
            }
            
            // Add gold to player inventory
            InventorySystem inventory = FindObjectOfType<InventorySystem>();
            if (inventory != null)
            {
                inventory.AddGold(goldReward);
            }
        }
        
        // Disable movement and combat
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        // Disable combatant component
        if (combatant != null)
        {
            combatant.enabled = false;
        }
        
        // Destroy after animation
        if (animator != null)
        {
            Destroy(gameObject, 2f); // Give time for death animation
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }
    
    // Called by Combatant component when it's the enemy's turn
    public void OnTurnStarted()
    {
        if (animator != null)
        {
            animator.SetTrigger("TurnStart");
        }
        
        Debug.Log($"{gameObject.name} started their turn");
    }
    
    // Called by Combatant component when the enemy's turn ends
    public void OnTurnEnded()
    {
        if (animator != null)
        {
            animator.SetTrigger("TurnEnd");
        }
        
        Debug.Log($"{gameObject.name} ended their turn");
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
