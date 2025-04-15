using UnityEngine;
using Component;
using Component.Interfaces;

using Utils; 
public enum PlayerEvent
{
    Idle,
    Attack,
    Jump,
    Die,
    Hit,
}

public class Player : MonoBehaviour, IDamageable
{
    [AutoRequire]
    public HealthComponent healthComponent;

    [AutoRequire]
    public AttackComponent attackComponent;
    
    [AutoRequire]
    public AnimationComponent animationComponent;
    
    [AutoRequire]
    public MoveComponent movementComponent;
    [AutoRequire]
    public JumpComponent jumpComponent;
    
    [AutoRequire]
    public InputComponent inputComponent;

    private StateMachine<Player, PlayerEvent> stateMachine;
    public delegate void ButtonPressedHandler();
    public event ButtonPressedHandler OnJumpButtonPressed;
    public event ButtonPressedHandler OnAttackButtonPressed;

    private void Awake()
    {
        ComponentInjector.InjectComponents(this);
        StateMachine<Player,PlayerEvent>.DebugMode = true;
        // Register input events
        inputComponent.OnFirePerformed += () =>
        {
            OnAttackButtonPressed?.Invoke();
        };
        inputComponent.OnJumpPerformed += () =>
        {
            OnJumpButtonPressed?.Invoke();
        };
    }
    
    
 
    private void Start()
    {
        // Here we configure the event mappings. These mappings are provided externally.
        stateMachine = new StateMachine<Player,PlayerEvent>(this, sm =>
        {
            sm.AddEventMapping(PlayerEvent.Idle,    () => sm.ChangeState<PlayerIdleState>());
            sm.AddEventMapping(PlayerEvent.Attack, () => sm.ChangeState<PlayerAttackState>());
          //  sm.AddEventMapping(PlayerEvent.Jump,   () => sm.ChangeState<PlayerJumpState>());
            sm.AddEventMapping(PlayerEvent.Hit,    () => sm.ChangeState<PlayerHitState>());
          
        });

        // Register states
        stateMachine.AddState(new PlayerIdleState());
        stateMachine.AddState(new PlayerAttackState());
        stateMachine.AddState(new PlayerHitState());
        
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

    public void TakeDamage(float damage)
    {
        healthComponent.TakeDamage(damage);
        stateMachine.ProcessEvent(PlayerEvent.Hit);

    }
    
    public void OnHit()
    {
 
    }

    public void Die()
    {
        stateMachine.ProcessEvent(PlayerEvent.Die);
    }
}