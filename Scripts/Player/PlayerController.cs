using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
    
    [Header("Combat")]
    public int attackDamage = 20;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;
    
    [Header("UI References")]
    public Slider healthBar;
    public Text levelText;
    public Text experienceText;
    
    // Private variables
    private Rigidbody2D rb;
    private Animator animator;
    private float lastAttackTime;
    private bool isSprinting;
    
    // Turn-based combat components
    private Combatant combatant;
    private GridMovement gridMovement;
    private TurnManager turnManager;
    
    // Combat state
    private bool isInCombat = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        
        // Get turn-based components
        combatant = GetComponent<Combatant>();
        gridMovement = GetComponent<GridMovement>();
        turnManager = FindObjectOfType<TurnManager>();
        
        UpdateUI();
    }
    
    void Update()
    {
        if (isInCombat)
        {
            HandleCombatInput();
        }
        else
        {
            HandleExplorationInput();
        }
        
        UpdateUI();
    }
    
    void HandleExplorationInput()
    {
        // Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        
        // Sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        
        // Apply movement
        rb.velocity = movement * currentSpeed;
        
        // Update animations
        if (animator != null)
        {
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetBool("IsSprinting", isSprinting);
        }
        
        // Check for enemies to start combat
        CheckForEnemies();
    }
    
    void HandleCombatInput()
    {
        // In combat, movement is handled by GridMovement
        // Just check for attack input
        if (Input.GetMouseButtonDown(0) && combatant != null && combatant.CanAttack())
        {
            AttemptAttack();
        }
    }
    
    void CheckForEnemies()
    {
        // Check if enemies are nearby to start combat
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 3f, enemyLayer);
        
        if (nearbyEnemies.Length > 0 && turnManager != null && !turnManager.isInCombat)
        {
            // Start combat
            StartCombat();
        }
    }
    
    void StartCombat()
    {
        isInCombat = true;
        
        // Stop movement
        rb.velocity = Vector2.zero;
        
        // Start turn-based combat
        if (turnManager != null)
        {
            turnManager.StartCombat();
        }
        
        Debug.Log("Combat started!");
    }
    
    void AttemptAttack()
    {
        if (combatant == null || !combatant.CanAttack()) return;
        
        // Find targets in attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        
        if (hitEnemies.Length > 0)
        {
            // Attack the first enemy in range
            Combatant target = hitEnemies[0].GetComponent<Combatant>();
            if (target != null)
            {
                Attack(target);
            }
        }
        else
        {
            Debug.Log("No enemies in attack range!");
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
        
        // Mark attack as completed
        if (combatant != null)
        {
            combatant.OnAttackCompleted();
        }
        
        Debug.Log($"Attacked {target.name} for {attackDamage} damage!");
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        
        UpdateUI();
    }
    
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateUI();
    }
    
    public void GainExperience(int exp)
    {
        experience += exp;
        
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
        
        UpdateUI();
    }
    
    void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel = (int)(experienceToNextLevel * 1.5f);
        
        // Increase stats
        maxHealth += 20;
        currentHealth = maxHealth;
        attackDamage += 5;
        
        // Play level up effect
        Debug.Log($"Level Up! You are now level {level}!");
    }
    
    void Die()
    {
        Debug.Log("Player died!");
        // Add respawn logic here
        currentHealth = maxHealth;
        transform.position = Vector3.zero; // Reset position
        
        // End combat if in combat
        if (isInCombat && turnManager != null)
        {
            turnManager.EndCombat();
            isInCombat = false;
        }
    }
    
    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
        
        if (levelText != null)
        {
            levelText.text = $"Level: {level}";
        }
        
        if (experienceText != null)
        {
            experienceText.text = $"XP: {experience}/{experienceToNextLevel}";
        }
    }
    
    // Called when combat ends
    public void OnCombatEnded()
    {
        isInCombat = false;
        
        // Re-enable exploration movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        Debug.Log("Combat ended, returning to exploration mode");
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw combat detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
