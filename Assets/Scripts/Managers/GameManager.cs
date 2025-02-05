using UnityEngine;
using Utils;

public class GameManager : SingletonMonoBehavior<GameManager>{
    
    public enum GameState { Loading, Playing, Paused, GameOver }
    
    [SerializeField] private GameState currentState = GameState.Loading;
    
    public GameState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }
    
    
   

}
