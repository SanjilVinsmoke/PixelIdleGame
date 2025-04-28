using UnityEngine;
using Component;
using Component.Interfaces;
using Utils;                         // For AutoRequireAttribute

public enum PlayerEvent
{
    Idle,
    Move,
    Jump,
    Fall,
    Attack,
    Dash,
    
    Hit,
    Die
}

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] public  bool debugMode = false; // Enable debug mode for state machine
    // Auto-injected components
    [AutoRequire] public InputComponent     inputComponent;
    [AutoRequire] public MoveComponent      movementComponent;
    [AutoRequire] public JumpComponent      jumpComponent;
    [AutoRequire] public AttackComponent    attackComponent;
    [AutoRequire] public DashComponent      dashComponent;
    [AutoRequire] public HealthComponent    healthComponent;
    [AutoRequire] public AnimationComponent animationComponent;

    private StateMachine<Player, PlayerEvent> stateMachine;

    private void Awake()
    {
        ComponentInjector.InjectComponents(this);
        StateMachine<Player, PlayerEvent>.DebugMode = debugMode;
    }

    private void Start()
    {
        // Configure state machine event-to-state mappings
        stateMachine = new StateMachine<Player, PlayerEvent>(this, sm =>
        {
            sm.AddEventMapping(PlayerEvent.Idle,   () => sm.ChangeState<PlayerIdleState>());
            sm.AddEventMapping(PlayerEvent.Move,   () => sm.ChangeState<PlayerMoveState>());
            sm.AddEventMapping(PlayerEvent.Jump,   () => sm.ChangeState<PlayerJumpState>());
            sm.AddEventMapping(PlayerEvent.Attack, () => sm.ChangeState<PlayerAttackState>());
            sm.AddEventMapping(PlayerEvent.Dash,   () => sm.ChangeState<PlayerDashState>());
            sm.AddEventMapping(PlayerEvent.Fall,   () => sm.ChangeState<PlayerFallState>());
            //sm.AddEventMapping(PlayerEvent.Hit,    () => sm.ChangeState<PlayerHitState>());
            //sm.AddEventMapping(PlayerEvent.Die,    () => sm.ChangeState<PlayerDieState>());
        });

        // Register state instances
        stateMachine.AddState(new PlayerIdleState());
        stateMachine.AddState(new PlayerMoveState());
        stateMachine.AddState(new PlayerJumpState());
        stateMachine.AddState(new PlayerAttackState());
        stateMachine.AddState(new PlayerDashState());
        stateMachine.AddState(new PlayerFallState());
      //  stateMachine.AddState(new PlayerHitState());
       // stateMachine.AddState(new PlayerDieState());

        // Start in Idle
        stateMachine.SetInitialState<PlayerIdleState>();
    }

    private void Update()
    {
        // check debug mode
        
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void TakeDamage(float damage)
    {
        healthComponent.TakeDamage(damage);

    }

    public void Die()
    {
        Debug.Log("Player died");
        // Optionally play death animation, disable controls, etc.
    }
}
