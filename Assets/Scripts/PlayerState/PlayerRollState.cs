using Constant;
using UnityEngine;
using Utils;

[StateDebugColor(StateDebugColorAttribute.UnityColor.Green)]
[StateDescription("Handles player rolling on the ground using RollComponent.")]
public class PlayerRollState : BaseState<Player, PlayerEvent>
{
    private RollComponent rollComponent;
    private const float DeadZone = 0.1f;

    public override void Initialize(Player owner, StateMachine<Player, PlayerEvent> stateMachine)
    {
        base.Initialize(owner, stateMachine);
        // Get the RollComponent from the owner GameObject
        rollComponent = owner.GetComponent<RollComponent>();
        if (rollComponent == null)
        {
            Debug.LogError("PlayerRollState requires a RollComponent on the Player GameObject.", owner);
        }
    }

    public override void Enter()
    {
        base.Enter();

        if (rollComponent != null && rollComponent.StartRoll())
        {
            // Subscribe to the roll end event
            rollComponent.OnRollEnd += HandleRollEnd;
            // Play the roll animation
            owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.ROLL); // Use the correct animation name constant
        }
        else
        {
            // If roll couldn't start (e.g., cooldown), immediately transition back
            Debug.LogWarning("Roll could not be started. Transitioning back.");
            HandleRollEnd(); // Transition immediately
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // Check if the player is no longer grounded during the roll
        if (rollComponent != null && rollComponent.IsRolling && !owner.jumpComponent.isGrounded)
        {
            // Interrupt the roll and transition to Fall state
            rollComponent.InterruptRoll(); // This will trigger OnRollEnd via EndRoll()
            stateMachine.ProcessEvent(PlayerEvent.Fall);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Ensure we unsubscribe if the state is exited prematurely
        if (rollComponent != null)
        {
            rollComponent.OnRollEnd -= HandleRollEnd;
            // Optional: Ensure roll is stopped if exiting for reasons other than completion
            // rollComponent.InterruptRoll(); // Be careful with this, might cause issues if already ended normally
        }
    }

    private void HandleRollEnd()
    {
        // Unsubscribe after handling
        if (rollComponent != null)
        {
             rollComponent.OnRollEnd -= HandleRollEnd;
        }

        // Check if still grounded before deciding next state
        if (!owner.jumpComponent.isGrounded)
        {
            stateMachine.ProcessEvent(PlayerEvent.Fall);
            return; // Exit early, already falling
        }

        // Transition back to Idle or Move based on input after roll completes
        if (Mathf.Abs(owner.inputComponent.MoveVector.x) > DeadZone)
            stateMachine.ProcessEvent(PlayerEvent.Move);
        else
            stateMachine.ProcessEvent(PlayerEvent.Idle);
    }
}