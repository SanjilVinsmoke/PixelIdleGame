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
    Roll,
    Hit,
    Death,
    DownSmash,
}

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] public  bool debugMode = false;

     public ParticleSystem dustEffectPrefab;
    
    // Enable debug mode for state machine
    // Auto-injected components
    [AutoRequire] public InputComponent     inputComponent;
    [AutoRequire] public MoveComponent      movementComponent;
    [AutoRequire] public JumpComponent      jumpComponent;
    [AutoRequire] public AttackComponent    attackComponent;
    [AutoRequire] public DashComponent      dashComponent;
    [AutoRequire] public HealthComponent    healthComponent;
    [AutoRequire] public AnimationComponent animationComponent;
    [AutoRequire] public RollComponent      rollComponent;
    [AutoRequire] public SmashComponent     smashComponent;
    
    
    private StateMachine<Player, PlayerEvent> stateMachine;
    
    
    
    // Current State
    public System.Type CurrentState => stateMachine.CurrentState?.GetType();

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
            sm.AddEventMapping(PlayerEvent.Roll,   () => sm.ChangeState<PlayerRollState>());
            sm.AddEventMapping(PlayerEvent.DownSmash, () => sm.ChangeState<PlayerDownSmashState>());
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
        stateMachine.AddState(new PlayerRollState());
        stateMachine.AddState(new PlayerDownSmashState());
      //  stateMachine.AddState(new PlayerHitState());
       // stateMachine.AddState(new PlayerDieState());

        // Start in Idle
        stateMachine.SetInitialState<PlayerIdleState>();
    }

    private void Update()
    {
        // check debug mode
        
        stateMachine.Update();
        Debug.Log($"Current State: {CurrentState}");
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
