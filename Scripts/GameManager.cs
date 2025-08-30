using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Systems")]
    public PlayerController player;
    public InventorySystem inventorySystem;
    public QuestSystem questSystem;
    
    [Header("Game Settings")]
    public bool isPaused = false;
    public bool isGameOver = false;
    
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;
    
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Find references if not set
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        
        if (inventorySystem == null)
            inventorySystem = FindObjectOfType<InventorySystem>();
        
        if (questSystem == null)
            questSystem = FindObjectOfType<QuestSystem>();
        
        // Initialize game state
        ResumeGame();
        ShowMainMenu();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Pause/Resume with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }
    
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
    
    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
    
    public void StartNewGame()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        ResumeGame();
    }
    
    // Helper methods for other systems
    public void OnEnemyKilled()
    {
        if (questSystem != null)
        {
            questSystem.OnEnemyKilled();
        }
    }
    
    public void OnItemCollected(string itemName)
    {
        if (questSystem != null)
        {
            questSystem.OnItemCollected(itemName);
        }
    }
    
    public void SaveGame()
    {
        // Basic save system - you can expand this
        PlayerPrefs.SetInt("PlayerLevel", player.level);
        PlayerPrefs.SetInt("PlayerExperience", player.experience);
        PlayerPrefs.SetInt("PlayerHealth", player.currentHealth);
        PlayerPrefs.SetInt("PlayerMaxHealth", player.maxHealth);
        PlayerPrefs.SetInt("PlayerDamage", player.attackDamage);
        
        PlayerPrefs.Save();
        Debug.Log("Game saved!");
    }
    
    public void LoadGame()
    {
        // Basic load system - you can expand this
        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            player.level = PlayerPrefs.GetInt("PlayerLevel");
            player.experience = PlayerPrefs.GetInt("PlayerExperience");
            player.currentHealth = PlayerPrefs.GetInt("PlayerHealth");
            player.maxHealth = PlayerPrefs.GetInt("PlayerMaxHealth");
            player.attackDamage = PlayerPrefs.GetInt("PlayerDamage");
            
            player.UpdateUI();
            Debug.Log("Game loaded!");
        }
    }
}
