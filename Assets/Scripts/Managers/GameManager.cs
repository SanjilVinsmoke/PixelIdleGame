using Managers;
using UnityEngine;
using Utils;
using Utils.Managers;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    public enum GameState { Loading, Playing, Paused, GameOver }
    [SerializeField]
    private GameState currentState = GameState.Loading;
    
    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private PlayerAbilityManager abilityManager;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private MapManager mapManager;
    
    [Header("References")]
    [SerializeField] private Player player;

    public GameState CurrentState => currentState;

    protected override void InitializeSingleton()
    {
        Debug.Log("GameManager Initialized");
        ChangeState(GameState.Playing);
    }

    private void Start()
    {
        // Auto-find references if missing
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        if (waveManager == null) waveManager = FindObjectOfType<WaveManager>();
        if (abilityManager == null) abilityManager = FindObjectOfType<PlayerAbilityManager>();
        if (saveManager == null) saveManager = FindObjectOfType<SaveManager>();
        if (mapManager == null) mapManager = FindObjectOfType<MapManager>();
        
        if (player == null) player = FindObjectOfType<Player>();
        
        // Try to load game on start if save exists
        if (saveManager != null && saveManager.HasSaveFile())
        {
           // saveManager.LoadGame(); // Uncomment if we want auto-load on start
        }

        if (currentState == GameState.Loading)
        {
            ChangeState(GameState.Playing);
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        Debug.Log($"Game State changed: {currentState} → {newState}");
        currentState = newState;

        switch (currentState)
        {
            case GameState.Playing:
              //  waveManager?.StartSpawning();
                Time.timeScale = 1;
                break;
            case GameState.Paused:
                Time.timeScale = 0;
                break;
            case GameState.GameOver:
              //  uiManager?.ShowGameOverScreen();
                break;
        }
    }

    public void SaveGame()
    {
        if (saveManager != null)
        {
            saveManager.SaveGame();
        }
        else
        {
            Debug.LogWarning("SaveManager not found!");
        }
    }

    public void LoadGame()
    {
        if (saveManager != null)
        {
            saveManager.LoadGame();
        }
         else
        {
            Debug.LogWarning("SaveManager not found!");
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        //waveManager?.ResetWaves();
        //player?.ResetPlayer();
        ChangeState(GameState.Playing);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        // Optional: Save on quit
        // SaveGame(); 
        Application.Quit();
    }
}
