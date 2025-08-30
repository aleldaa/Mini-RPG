using UnityEngine;
using System.Collections.Generic;

public class GridMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 1f;
    public int moveRange = 3;
    public LayerMask obstacleLayer;
    public LayerMask enemyLayer;
    
    [Header("Visual")]
    public GameObject moveTilePrefab;
    public Color validMoveColor = Color.green;
    public Color invalidMoveColor = Color.red;
    
    // Private variables
    private bool isMovementEnabled = false;
    private List<GameObject> moveTiles = new List<GameObject>();
    private Combatant combatant;
    private TurnManager turnManager;
    private Vector3 targetPosition;
    private bool isMoving = false;
    
    void Start()
    {
        combatant = GetComponent<Combatant>();
        turnManager = FindObjectOfType<TurnManager>();
    }
    
    void Update()
    {
        if (!isMovementEnabled || !combatant.CanMove() || isMoving) return;
        
        HandleInput();
    }
    
    void HandleInput()
    {
        // Grid-based movement with arrow keys or WASD
        Vector3 moveDirection = Vector3.zero;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            moveDirection = Vector3.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            moveDirection = Vector3.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            moveDirection = Vector3.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            moveDirection = Vector3.right;
        
        if (moveDirection != Vector3.zero)
        {
            Vector3 newPosition = transform.position + moveDirection * gridSize;
            if (IsValidMovePosition(newPosition))
            {
                MoveToPosition(newPosition);
            }
        }
        
        // Right click to cancel movement
        if (Input.GetMouseButtonDown(1))
        {
            CancelMovement();
        }
    }
    
    public void EnableMovement()
    {
        isMovementEnabled = true;
        ShowMoveRange();
    }
    
    public void DisableMovement()
    {
        isMovementEnabled = false;
        HideMoveRange();
    }
    
    void ShowMoveRange()
    {
        if (moveTilePrefab == null) return;
        
        // Clear existing tiles
        HideMoveRange();
        
        // Show valid move positions
        for (int x = -moveRange; x <= moveRange; x++)
        {
            for (int y = -moveRange; y <= moveRange; y++)
            {
                // Check if within move range
                if (Mathf.Abs(x) + Mathf.Abs(y) <= moveRange)
                {
                    Vector3 tilePosition = transform.position + new Vector3(x * gridSize, y * gridSize, 0);
                    
                    if (IsValidMovePosition(tilePosition))
                    {
                        GameObject tile = Instantiate(moveTilePrefab, tilePosition, Quaternion.identity);
                        tile.GetComponent<SpriteRenderer>().color = validMoveColor;
                        moveTiles.Add(tile);
                    }
                }
            }
        }
    }
    
    void HideMoveRange()
    {
        foreach (GameObject tile in moveTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        moveTiles.Clear();
    }
    
    bool IsValidMovePosition(Vector3 position)
    {
        // Check if position is within move range
        float distance = Vector3.Distance(transform.position, position);
        if (distance > moveRange * gridSize)
            return false;
        
        // Check if position is occupied by obstacles
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(position, gridSize * 0.4f, obstacleLayer);
        if (obstacles.Length > 0)
            return false;
        
        // Check if position is occupied by other combatants
        Collider2D[] combatants = Physics2D.OverlapCircleAll(position, gridSize * 0.4f, enemyLayer);
        if (combatants.Length > 0)
            return false;
        
        return true;
    }
    
    void MoveToPosition(Vector3 position)
    {
        if (isMoving) return;
        
        targetPosition = position;
        StartCoroutine(MoveCoroutine());
    }
    
    System.Collections.IEnumerator MoveCoroutine()
    {
        isMoving = true;
        
        // Smooth movement to target position
        float moveTime = 0.2f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        
        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;
            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
        isMoving = false;
        
        // Movement completed
        if (combatant != null)
        {
            combatant.OnMoveCompleted();
        }
        
        // Hide move range after moving
        HideMoveRange();
    }
    
    void CancelMovement()
    {
        HideMoveRange();
        // Movement is cancelled, but turn continues
    }
    
    public void SetMoveRange(int newMoveRange)
    {
        moveRange = newMoveRange;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw move range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Vector3.one * moveRange * gridSize * 2);
        
        // Draw grid cells
        Gizmos.color = Color.gray;
        for (int x = -moveRange; x <= moveRange; x++)
        {
            for (int y = -moveRange; y <= moveRange; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= moveRange)
                {
                    Vector3 cellPos = transform.position + new Vector3(x * gridSize, y * gridSize, 0);
                    Gizmos.DrawWireCube(cellPos, Vector3.one * gridSize);
                }
            }
        }
    }
}
