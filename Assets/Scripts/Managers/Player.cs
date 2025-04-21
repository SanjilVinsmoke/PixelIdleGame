using UnityEngine;
using Component;
using Component.Interfaces;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using Utils; 
public enum PlayerEvent
{
    Idle,
    Attack,
    Move,
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
    public delegate void MoveButtonPressedHandler(InputAction.CallbackContext context);
    public event ButtonPressedHandler OnJumpButtonPressed;
    public event ButtonPressedHandler OnAttackButtonPressed;
    public event MoveButtonPressedHandler OnMoveButtonPressed;

    private void Awake()
    {
        ComponentInjector.InjectComponents(this);
        StateMachine<Player,PlayerEvent>.DebugMode = true;
        
        // Register input events with named methods for proper cleanup
        inputComponent.OnFirePerformed += HandleFireInput;
        inputComponent.OnJumpPerformed += HandleJumpInput;
        inputComponent.OnMovePerformed += HandleMoveInput;
        
        // Connect jump component landing event
        jumpComponent.OnLanded += HandleJumpLanding;
    }
    
    private void HandleFireInput() => OnAttackButtonPressed?.Invoke();
    private void HandleJumpInput() => OnJumpButtonPressed?.Invoke();
    private void HandleMoveInput(InputAction.CallbackContext context) => OnMoveButtonPressed?.Invoke(context);
    
    private void HandleJumpLanding()
    {
        // Only transition to idle if we're currently in jump state
        if (stateMachine.CurrentState is PlayerJumpState)
            stateMachine.ProcessEvent(PlayerEvent.Idle);
    }
 
    private void Start()
    {
        // Here we configure the event mappings. These mappings are provided externally.
        stateMachine = new StateMachine<Player,PlayerEvent>(this, sm =>
        {
            sm.AddEventMapping(PlayerEvent.Idle,    () => sm.ChangeState<PlayerIdleState>());
            sm.AddEventMapping(PlayerEvent.Attack, () => sm.ChangeState<PlayerAttackState>());
            sm.AddEventMapping(PlayerEvent.Move,   () => sm.ChangeState<PlayerMoveState>());
            sm.AddEventMapping(PlayerEvent.Jump,   () => sm.ChangeState<PlayerJumpState>());
            sm.AddEventMapping(PlayerEvent.Hit,    () => sm.ChangeState<PlayerHitState>());
          
        });

        // Register states
        stateMachine.AddState(new PlayerIdleState());
        stateMachine.AddState(new PlayerAttackState());
        stateMachine.AddState(new PlayerHitState());
        stateMachine.AddState(new PlayerMoveState());
        stateMachine.AddState(new PlayerJumpState());
        
        stateMachine.SetInitialState<PlayerIdleState>();
    }

    private void Update()
    {
        stateMachine.Update();

        
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions to prevent memory leaks
        if (inputComponent != null)
        {
            inputComponent.OnFirePerformed -= HandleFireInput;
            inputComponent.OnJumpPerformed -= HandleJumpInput;
            inputComponent.OnMovePerformed -= HandleMoveInput;
        }
        
        if (jumpComponent != null)
        {
            jumpComponent.OnLanded -= HandleJumpLanding;
        }
        
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