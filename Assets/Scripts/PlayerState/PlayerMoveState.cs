using Constant;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

/// <summary>
/// Player movement state: listens to move input events and drives the MoveComponent.
/// </summary>
[StateDebugColor(StateDebugColorAttribute.UnityColor.Blue)]
[StateDescription("Handles player horizontal movement. Subscribes to input events and moves via MoveComponent.")]
public class PlayerMoveState : BaseState<Player, PlayerEvent>
{
    private float horizontalInput;
    private const float DeadZone = 0.1f;

    public override void Enter()
    {
        base.Enter();
    horizontalInput = owner.inputComponent.MoveVector.x;
        if (Mathf.Abs(horizontalInput) < DeadZone)
            horizontalInput = 0f;
        // Subscribe to relevant input events
        owner.inputComponent.OnMovePerformed   += HandleMovePerformed;
        owner.inputComponent.OnJumpPerformed   += HandleJumpPressed;
        owner.inputComponent.OnAttackPerformed += HandleAttackPressed;
        owner.inputComponent.OnDashPerformed   += HandleDashPressed;

        // Play run animation if available
        owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.RUN);
    }

    public override void Update()
    {
        base.Update();

        // Drive movement each frame
        owner.movementComponent.Move(horizontalInput);
    }

    public override void Exit()
    {
        base.Exit();

        // Unsubscribe from input events
        owner.inputComponent.OnMovePerformed   -= HandleMovePerformed;
        owner.inputComponent.OnJumpPerformed   -= HandleJumpPressed;
        owner.inputComponent.OnAttackPerformed -= HandleAttackPressed;
        owner.inputComponent.OnDashPerformed   -= HandleDashPressed;

        owner.movementComponent.Move(0f);
    }

    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            horizontalInput = context.ReadValue<Vector2>().x;
            if (Mathf.Abs(horizontalInput) < DeadZone)
                horizontalInput = 0f;
        }
        else if (context.canceled)
        {
            // Stop and go back to idle on cancel
            horizontalInput = 0f;
            stateMachine.ProcessEvent(PlayerEvent.Idle);
        }
    }

    private void HandleJumpPressed()   => stateMachine.ProcessEvent(PlayerEvent.Jump);
    private void HandleAttackPressed() => stateMachine.ProcessEvent(PlayerEvent.Attack);
    private void HandleDashPressed()   => stateMachine.ProcessEvent(PlayerEvent.Dash);
}