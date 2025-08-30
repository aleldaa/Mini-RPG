using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    public float spawnRadius = 20f;
    public float spawnInterval = 5f;
    public float minSpawnDistance = 5f;
    
    [Header("Spawn Area")]
    public Transform player;
    public LayerMask obstacleLayer;
    
    private int currentEnemyCount = 0;
    private float lastSpawnTime;
    
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Spawn initial enemies
        for (int i = 0; i < maxEnemies / 2; i++)
        {
            SpawnEnemy();
        }
    }
    
    void Update()
    {
        if (currentEnemyCount < maxEnemies && Time.time >= lastSpawnTime + spawnInterval)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }
    
    void SpawnEnemy()
    {
        if (enemyPrefab == null || player == null) return;
        
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        if (spawnPosition != Vector3.zero)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Set up enemy
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // Subscribe to enemy death event
                StartCoroutine(MonitorEnemy(enemy));
            }
            
            currentEnemyCount++;
        }
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        for (int attempts = 0; attempts <30; attempts++)
        {
            // Get random position within spawn radius
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, spawnRadius);
            Vector3 spawnPos = player.position + new Vector3(randomCircle.x, randomCircle.y, 0);
            
            // Check if position is valid (not inside obstacles)
            Collider2D[] obstacles = Physics2D.OverlapCircleAll(spawnPos, 1f, obstacleLayer);
            if (obstacles.Length == 0)
            {
                return spawnPos;
            }
        }
        
        return Vector3.zero; // Could not find valid position
    }
    
    IEnumerator MonitorEnemy(GameObject enemy)
    {
        yield return new WaitUntil(() => enemy == null);
        currentEnemyCount--;
        
        // Notify quest system
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyKilled();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // Draw spawn area
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minSpawnDistance);
        }
    }
}
