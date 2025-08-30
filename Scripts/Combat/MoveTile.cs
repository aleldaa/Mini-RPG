using UnityEngine;
using UnityEngine.EventSystems;

public class MoveTile : MonoBehaviour, IPointerClickHandler
{
    [Header("Tile Settings")]
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    public bool isValidMove = true;
    
    private SpriteRenderer spriteRenderer;
    private GridMovement gridMovement;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridMovement = FindObjectOfType<GridMovement>();
        
        // Set initial color
        UpdateColor();
    }
    
    public void SetValidMove(bool valid)
    {
        isValidMove = valid;
        UpdateColor();
    }
    
    void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isValidMove ? validColor : invalidColor;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isValidMove) return;
        
        // Move the player to this position
        if (gridMovement != null)
        {
            gridMovement.MoveToPosition(transform.position);
        }
    }
    
    // Called when the tile is no longer needed
    public void DestroyTile()
    {
        Destroy(gameObject);
    }
}
