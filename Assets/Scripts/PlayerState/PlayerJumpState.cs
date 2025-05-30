using Constant;
using UnityEngine;
using Utils;
using UnityEngine.InputSystem;

[StateDebugColor(StateDebugColorAttribute.UnityColor.Cyan)]
[StateDescription("Handles player jumping: initial, double-jump, coyote time, in-air control, and landing.")]
public class PlayerJumpState : BaseState<Player, PlayerEvent>
{
    private float horizontalInput;
    private const float DeadZone = 0.1f;
    private Rigidbody2D rb;
    public override void Enter()
    {
        base.Enter();

        rb = owner.GetComponent<Rigidbody2D>();
        // Perform initial jump
        owner.jumpComponent.Jump();

        // Capture input for air control
        horizontalInput = owner.inputComponent.MoveVector.x;

        // Subscribe to input events
        owner.inputComponent.OnMovePerformed   += HandleMove;
        owner.inputComponent.OnJumpPerformed   += HandleJumpPressed;
        owner.inputComponent.OnJumpCanceled    += HandleJumpCanceled;
        owner.inputComponent.OnDashPerformed   += HandleDashPressed;
        // Subscribe to landing event
        owner.jumpComponent.OnLanded           += HandleLanded;

        owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.JUMP);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        owner.movementComponent.Move(horizontalInput);

        // **If we start falling, switch to FallState**
        if (rb.linearVelocity.y < 0f)
        {
            stateMachine.ProcessEvent(PlayerEvent.Fall);
        }
    }
    public override void Exit()
    {
        base.Exit();

        // Unsubscribe from input events
        owner.inputComponent.OnMovePerformed   -= HandleMove;
        owner.inputComponent.OnJumpPerformed   -= HandleJumpPressed;
        owner.inputComponent.OnJumpCanceled    -= HandleJumpCanceled;
        owner.jumpComponent.OnLanded           -= HandleLanded;
    }

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            horizontalInput = ctx.ReadValue<Vector2>().x;
        else if (ctx.canceled)
            horizontalInput = 0f;
    }

    private void HandleJumpPressed()
    {
        owner.jumpComponent.Jump();
    }

    private void HandleJumpCanceled()
    {
        owner.jumpComponent.EndJump();
    }

    private void HandleLanded()
    {
        // When player lands, transition to Move or Idle based on input
        if (Mathf.Abs(owner.inputComponent.MoveVector.x) > DeadZone)
            stateMachine.ProcessEvent(PlayerEvent.Move);
        else
            stateMachine.ProcessEvent(PlayerEvent.Idle);
    }
    
    private void HandleDashPressed()
    {
        if (owner.CurrentState == typeof(PlayerJumpState) || owner.CurrentState == typeof(PlayerFallState))
        {
            // If in Jump or Fall state, process the dash event
            stateMachine.ProcessEvent(PlayerEvent.Dash);
        }
        
       
    }
}
