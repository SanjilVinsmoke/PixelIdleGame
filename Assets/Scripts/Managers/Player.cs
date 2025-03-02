using UnityEngine;
using Component;
using Component.Interfaces;

using Utils; 
public enum PlayerEvent
{
    Idle,
    Attack,
    Die,
    Hit,
}

public class Player : MonoBehaviour
{
    [AutoRequire]
    public HealthComponent healthComponent;

    [AutoRequire]
    public AttackComponent attackComponent;

    private StateMachine<Player, PlayerEvent> stateMachine;
    public delegate void UIButtonPressedHandler();
    public event UIButtonPressedHandler OnJumpButtonPressed;
    public event UIButtonPressedHandler OnAttackButtonPressed;

    private void Awake()
    {
        ComponentInjector.InjectComponents(this);
        StateMachine<Player,PlayerEvent>.DebugMode = true;
    }
    
    public void InvokeAttack()
    {
        OnAttackButtonPressed?.Invoke();
    }

    private void Start()
    {
        // Here we configure the event mappings. These mappings are provided externally.
        stateMachine = new StateMachine<Player,PlayerEvent>(this, sm =>
        {
            sm.AddEventMapping(PlayerEvent.Idle,    () => sm.ChangeState<PlayerIdleState>());
            sm.AddEventMapping(PlayerEvent.Attack, () => sm.ChangeState<PlayerAttackState>());
            // sm.AddEventMapping(PlayerEvent.Die,    () => sm.ChangeState<PlayerDeathState>());
            // sm.AddEventMapping(PlyerEvent.Hit, () => sm.ChangeState<PlayerHitState>());
        });

        // Register states
        stateMachine.AddState(new PlayerIdleState());
        stateMachine.AddState(new PlayerAttackState());
        // stateMachine.AddState(new PlayerDeathState());
        // stateMachine.AddState(new PlayerHitState());
        stateMachine.SetInitialState<PlayerIdleState>();
    }

    private void Update()
    {
        stateMachine.Update();

        
    }

 
    private void OnDestroy()
    {
        stateMachine.Dispose();
    }
}