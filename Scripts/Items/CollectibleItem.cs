using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName;
    public string description;
    public ItemType itemType;
    public int value;
    public int quantity = 1;
    
    [Header("Visual Effects")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    
    [Header("Collection")]
    public float collectionRange = 2f;
    public LayerMask playerLayer;
    
    private Vector3 startPosition;
    private float bobTime;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Visual effects
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        bobTime += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(bobTime) * bobHeight;
        transform.position = startPosition + Vector3.up * bobOffset;
        
        // Check for player proximity
        CheckPlayerProximity();
    }
    
    void CheckPlayerProximity()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, collectionRange, playerLayer);
        
        if (player != null)
        {
            // Show collection prompt
            ShowCollectionPrompt();
            
            // Check for collection input
            if (Input.GetKeyDown(KeyCode.E))
            {
                CollectItem();
            }
        }
    }
    
    void ShowCollectionPrompt()
    {
        // You can add UI text here to show "Press E to collect"
        // For now, we'll just use debug log
        Debug.Log($"Press E to collect {itemName}");
    }
    
    void CollectItem()
    {
        // Create inventory item
        InventoryItem item = new InventoryItem(itemName, description, null, itemType, value);
        item.quantity = quantity;
        
        // Add to player inventory
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory != null)
        {
            if (inventory.AddItem(item))
            {
                // Notify quest system
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnItemCollected(itemName);
                }
                
                // Play collection effect
                PlayCollectionEffect();
                
                // Destroy the item
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory is full!");
            }
        }
    }
    
    void PlayCollectionEffect()
    {
        // You can add particle effects, sound, etc. here
        Debug.Log($"Collected {itemName}!");
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw collection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}
