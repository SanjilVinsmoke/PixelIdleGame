
using Managers;
using Test;
using UnityEngine;
using Utils;
using Utils.Managers;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    public enum GameState { Loading, Playing, Paused, GameOver }
    [SerializeField]
    private GameState currentState = GameState.Loading;
    
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Player player;

    public GameState CurrentState => currentState;

    protected override void InitializeSingleton()
    {
        Debug.Log("GameManager Initialized");
        ChangeState(GameState.Playing);
    }

    private void Start()
    {
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        if (waveManager == null) waveManager = FindObjectOfType<WaveManager>();
        if (player == null) player = FindObjectOfType<Player>();
        
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

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        //waveManager?.ResetWaves();
      //  player?.ResetPlayer();
        ChangeState(GameState.Playing);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}