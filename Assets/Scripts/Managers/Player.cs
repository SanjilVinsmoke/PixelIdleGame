using System;
using UnityEngine;
using Component;
using Component.Interfaces;
using PlayerState;
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

public class Player : MonoBehaviour, IDamageable
{
    
    [Header("Player Settings")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private  bool debugMode = false;
    public Vector2 LastHitDirection { get; set; }
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
    [AutoRequire] public HitComponent       hitComponent;   
    
    private StateMachine<Player, PlayerEvent> stateMachine;
    
    
    
    // Current State
    public Type CurrentState => stateMachine.CurrentState?.GetType();

    private void Awake()
    {       
        ComponentInjector.InjectComponents(this);
        animationComponent.animator = transform.GetChild(0).GetComponent<Animator>();
        StateMachine<Player, PlayerEvent>.DebugMode = debugMode;
        healthComponent.OnDeath += Die;
    }

    private void Start()
    {
        
        movementComponent.speed = playerData.moveSpeed;
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
            sm.AddEventMapping(PlayerEvent.Hit,    () => sm.ChangeState<PlayerHitState>());
            sm.AddEventMapping(PlayerEvent.Death, () => sm.ChangeState<PlayerSwimState>());
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
        stateMachine.AddState(new PlayerHitState());
        stateMachine.AddState(new PlayerSwimState());
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

    public void TakeDamage(float damage , Vector2 hitDirection)
    {
        if (hitComponent.IsInvincible || healthComponent.IsDead)
        {
           return;
        }
        healthComponent.TakeDamage(damage);
        LastHitDirection = hitDirection;
        if (!healthComponent.IsDead)
        {
            stateMachine.ChangeState<PlayerHitState>();
        }
        

    }

  

    public void Die()
    {
        Debug.Log("Player died");
        stateMachine.ProcessEvent(PlayerEvent.Death);
    }

    public void OnHit()
    {
        throw new NotImplementedException();
    }
}
