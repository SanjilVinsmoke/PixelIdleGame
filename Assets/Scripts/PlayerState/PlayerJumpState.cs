using Constant;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

[StateDescription("Player is jumping")]
[StateDebugColor(StateDebugColorAttribute.UnityColor.Cyan)]
public class PlayerJumpState : BaseState<Player, PlayerEvent>
{
    private float jumpStartTime;
    private float maxJumpDuration = 1.0f;
    private Vector2 moveInput;
    private bool hasLanded = false;

    public override void Enter()
    {
        base.Enter();
        jumpStartTime = Time.time;
        hasLanded = false;
        
        // Cache the current movement input on entry
        if (owner.inputComponent != null)
            moveInput = owner.inputComponent.MoveVector;
        
        // Register event handlers
        owner.OnAttackButtonPressed += HandleAttackPressed;
        owner.OnJumpButtonPressed += HandleJumpPressed;
        
        if (owner.jumpComponent != null)
            owner.jumpComponent.HandleJumpInput();
        
        if (owner.animationComponent != null)
            owner.animationComponent.PlayAnimation(AnimationName.PlayerAnimationNames.JUMP);
    }
    
    public override void Update()
    {
        base.Update();

    
        // Only handle non-physics updates here
        if (Time.time - jumpStartTime > maxJumpDuration && !hasLanded)
        {
            stateMachine.ProcessEvent(PlayerEvent.Idle);
        }
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        // Handle physics-based movement in FixedUpdate
        if (owner.movementComponent != null && owner.inputComponent != null)
        {
            Vector2 currentInput = owner.inputComponent.MoveVector;
            moveInput = currentInput;
            owner.movementComponent.Move(currentInput.x * 0.8f);
        }
    }
    
    private void HandleAttackPressed()
    {
        stateMachine.ProcessEvent(PlayerEvent.Attack);
    }
    
    private void HandleJumpPressed()
    {
        // Allow double jump if component supports it
        if (owner.jumpComponent != null)
            owner.jumpComponent.HandleJumpInput();
    }
    
    public override void Exit()
    {
        // Determine proper transition state based on cached movement
        if (moveInput.magnitude > 0.1f)
        {
            // If we still have movement input when exiting jump, 
            // directly go to move state next time
            owner.inputComponent.lastMoveDirection = moveInput;
        }
        
        // Unregister events to prevent memory leaks
        owner.OnAttackButtonPressed -= HandleAttackPressed;
        owner.OnJumpButtonPressed -= HandleJumpPressed;
        
        base.Exit();
    }
}
