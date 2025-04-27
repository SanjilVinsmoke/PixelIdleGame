using UnityEngine;

// Corrected: Inherit from BaseState<Player, PlayerEvent>
public class PlayerDashState : BaseState<Player, PlayerEvent>
{
    // Removed: private Player player;
    private DashComponent dashComponent;
    private float dashStartTime;

    // Removed: Custom constructor
    // public PlayerDashState(Player player, StateMachine stateMachine) : base(stateMachine)
    // {
    //     this.player = player;
    //     this.dashComponent = player.GetComponent<DashComponent>();
    //     if (dashComponent == null)
    //     {
    //         Debug.LogError("[PlayerDashState] DashComponent not found on Player!");
    //     }
    // }

    // Added: Initialize method override
    public override void Initialize(Player owner, StateMachine<Player, PlayerEvent> stateMachine)
    {
        base.Initialize(owner, stateMachine);
        // Access player instance via 'owner'
        this.dashComponent = owner.GetComponent<DashComponent>();
        if (dashComponent == null)
        {
            Debug.LogError("[PlayerDashState] DashComponent not found on Player!");
        }
    }

    public override void Enter()
    {
        base.Enter();
        if (dashComponent != null && dashComponent.CanDash)
        {
            dashComponent.PerformDash();
            dashStartTime = Time.time;
            // Optional: Trigger dash animation
            // owner.animationComponent?.SetTrigger("Dash"); // Use 'owner'
        }
        else
        {
            // Cannot dash, transition back immediately
            // Use 'owner' to access components
            // if (owner.jumpComponent.IsGrounded)
            // {
            //     stateMachine.ChangeState<PlayerIdleState>();
            // }
            // else
            // {
            //     // stateMachine.ChangeState<PlayerFallState>(); // Assuming a FallState exists
            //      stateMachine.ChangeState<PlayerIdleState>(); // Placeholder
            // }
        }
    }

    // Renamed: LogicUpdate to Update
    public override void Update()
    {
        base.Update();

        // Check if dash duration has ended
        if (dashComponent == null || !dashComponent.IsDashing)
        {
            // Transition to appropriate state after dash
            // Use 'owner' to access components
            // if (owner.jumpComponent.IsGrounded) // Assuming JumpComponent exists and tracks grounded state
            // {
            //     stateMachine.ChangeState<PlayerIdleState>();
            // }
            // else
            // {
            //     // Need a FallState or similar
            //     // stateMachine.ChangeState<PlayerFallState>();
            //     // For now, transition to Idle as a placeholder
            //      stateMachine.ChangeState<PlayerIdleState>();
            // }
        }
    }

    // Renamed: PhysicsUpdate to FixedUpdate
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // Dash movement is handled by DashComponent's coroutine
    }

    public override void Exit()
    {
        base.Exit();
        // Optional: Ensure dash effects are cleaned up if interrupted
        // Consider if CancelDash is needed here or if DashComponent handles it
        // dashComponent?.CancelDash();
    }
}