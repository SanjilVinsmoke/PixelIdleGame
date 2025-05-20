using Component;
using UnityEngine;

// Corrected: Inherit from BaseState<Player, PlayerEvent>
public class PlayerHitState : BaseState<Player, PlayerEvent>
{
    // Removed: private Player player;
    private HitComponent hitComponent;
    private float hitStartTime;

   

    // Added: Initialize method override
    public override void Initialize(Player owner, StateMachine<Player, PlayerEvent> stateMachine)
    {
        base.Initialize(owner, stateMachine);
        // Access player instance via 'owner'
        this.hitComponent = owner.GetComponent<HitComponent>();
        if (hitComponent == null)
        {
            Debug.LogError("[PlayerHitState] HitComponent not found on Player!");
        }
    }

    public override void Enter()
    {
        base.Enter();
        hitStartTime = Time.time;
        // Knockback is initiated by HitComponent.TakeHit (called from Player.TakeDamage)
        // Optional: Trigger hit animation
        // owner.animationComponent?.SetTrigger("Hit"); // Use 'owner'
        // Disable player input during hit state? (Handled by state logic)
    }

    // Renamed: LogicUpdate to Update to match BaseState
    public override void Update()
    {
        base.Update();

        // Check if knockback/invincibility duration has ended (HitComponent manages this)
        // if (hitComponent == null || !hitComponent.IsHit) // Check IsHit status from HitComponent
        // {
        //     // Transition back to a default state after hit recovery
        //     // Use 'owner' to access player components
        //     // if (owner.jumpComponent.IsGrounded) // Assuming JumpComponent exists on owner
        //     // {
        //     //     stateMachine.ChangeState<PlayerIdleState>();
        //     // }
        //     else
        //     {
        //         // Need a FallState or similar
        //         // stateMachine.ChangeState<PlayerFallState>();
        //         // For now, transition to Idle as a placeholder
        //          stateMachine.ChangeState<PlayerIdleState>();
        //     }
        // }
    }

    // Renamed: PhysicsUpdate to FixedUpdate to match BaseState
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // Knockback movement is handled by HitComponent's coroutine
    }

    public override void Exit()
    {
        base.Exit();
        // Ensure hit effects are cleaned up if state is exited prematurely (e.g., death)
        // HitComponent might handle this internally when IsHit becomes false
        // hitComponent?.CancelHitEffects();
    }

    // This state is entered via Player.TakeDamage -> stateMachine.ProcessEvent(PlayerEvent.Hit)
    // The actual hit application (knockback, invincibility) should happen in HitComponent.TakeHit
}