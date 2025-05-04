using Constant;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

[StateDebugColor(StateDebugColorAttribute.UnityColor.Purple)]
[StateDescription("Handles player falling: shows fall sprite + in-air control until landing.")]
public class PlayerFallState : BaseState<Player, PlayerEvent>
{
    private float horizontalInput;
    private float verticalInput;
    private Rigidbody2D rb;
    private const float DeadZone = 0.1f;

    public override void Enter()
    {
        base.Enter();
        rb = owner.GetComponent<Rigidbody2D>();
        horizontalInput = owner.inputComponent.MoveVector.x;
        verticalInput = owner.inputComponent.MoveVector.y;

        owner.inputComponent.OnMovePerformed += HandleMove;
        // Listen for the JumpPerformed too, so you can double-jump in FallState:
        owner.inputComponent.OnJumpPerformed += HandleJumpPressed;
        owner.inputComponent.OnJumpCanceled  += HandleJumpCanceled;
        owner.inputComponent.OnDashPerformed  += HandleDashPressed;
        owner.jumpComponent.OnLanded        += HandleLanded;

        owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.FALL);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        owner.movementComponent.Move(horizontalInput);
        // In your input handling method
        if (verticalInput < -0.5f) // Pressing down
        {
            stateMachine.ProcessEvent(PlayerEvent.DownSmash);
        }

        if (owner.jumpComponent.isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
            HandleLanded();
    }


    public override void Exit()
    {
        base.Exit();
        owner.inputComponent.OnMovePerformed -= HandleMove;
        owner.inputComponent.OnJumpPerformed -= HandleJumpPressed;
        owner.inputComponent.OnJumpCanceled  -= HandleJumpCanceled;
        owner.jumpComponent.OnLanded        -= HandleLanded;
    }

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            horizontalInput = ctx.ReadValue<Vector2>().x;
            verticalInput = ctx.ReadValue<Vector2>().y;
        }
        
        else if (ctx.canceled)
        {
            horizontalInput = 0f;
            verticalInput = 0f;
        }
        
        
    }

    private void HandleJumpPressed()
    {
        // allow double-jump from fall
        owner.jumpComponent.Jump();
        // also switch back into JumpState so they get the rising animation
        stateMachine.ProcessEvent(PlayerEvent.Jump);
    }

    private void HandleJumpCanceled()
    {
        owner.jumpComponent.EndJump();
    }

    private void HandleLanded()
    {
        // Unsub first so we don't re-enter
        owner.jumpComponent.OnLanded -= HandleLanded;

        // Transition based on horizontal input
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
