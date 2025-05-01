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
        rollComponent = owner.rollComponent;
        if (rollComponent == null)
        {
            Debug.LogError("PlayerRollState requires a RollComponent on the Player GameObject.", owner);
        }
    }

    public override void Enter()
    {
        base.Enter();

        if (rollComponent != null && rollComponent.StartRoll(owner.inputComponent.MoveVector))
        {
            // Subscribe to the roll end event
            rollComponent.OnRollEnd += HandleRollEnd;
            
            // Subscribe to input events that can interrupt the roll
            owner.inputComponent.OnJumpPerformed += HandleJumpInput;
            owner.inputComponent.OnAttackPerformed += HandleAttackInput;
            owner.inputComponent.OnMovePerformed += HandleMoveInput;
            
            // Play the roll animation
            owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.ROLL);
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
            
            // Unsubscribe from input events
            owner.inputComponent.OnJumpPerformed -= HandleJumpInput;
            owner.inputComponent.OnAttackPerformed -= HandleAttackInput;
            owner.inputComponent.OnMovePerformed -= HandleMoveInput;
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
    
    // Handle jump input during roll
    private void HandleJumpInput()
    {
        if (rollComponent != null && rollComponent.canInterruptRoll)
        {
            rollComponent.InterruptRoll();
            stateMachine.ProcessEvent(PlayerEvent.Jump);
        }
    }
    
    // Handle attack input during roll
    private void HandleAttackInput()
    {
        if (rollComponent != null && rollComponent.canInterruptRoll)
        {
            rollComponent.InterruptRoll();
            stateMachine.ProcessEvent(PlayerEvent.Attack);
        }
    }
    
    // Handle move input during roll
    private void HandleMoveInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // Only interrupt if there's significant input in a different direction
        if (rollComponent != null && rollComponent.canInterruptRoll)
        {
            Vector2 input = context.ReadValue<Vector2>();
            Vector2 rollDir = rollComponent.RollDirection;
            
            // Check if input is in a significantly different direction than the roll
            if (input.magnitude > DeadZone && Vector2.Dot(input.normalized, rollDir) < 0.5f)
            {
                rollComponent.InterruptRoll();
                stateMachine.ProcessEvent(PlayerEvent.Move);
            }
        }
    }
}