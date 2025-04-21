using Constant;
using UnityEngine;

public class PlayerMoveState: BaseState<Player, PlayerEvent>
{
    public override void Enter()
    {
        base.Enter();
        if (owner.inputComponent != null)
        {
            owner.inputComponent.OnFirePerformed += HandleAttackPressed;
            owner.inputComponent.OnJumpPerformed += HandleJumpPressed;
            
            // We're not subscribing to OnMovePerformed since we're using MoveVector directly
        }
        
        // Play the move animation if an animator exists
        if (owner.animationComponent != null)
        {
            owner.animationComponent.PlayAnimation(AnimationName.PlayerAnimationNames.RUN);
        }
    }

    public override void Update()
    {
        base.Update();
        
        // Use movement component to move the player based on input
        if (owner.movementComponent != null && owner.inputComponent != null)
        {
            Vector2 moveInput = owner.inputComponent.MoveVector;
            
            // Pass only the x component since your Move method accepts a float
            owner.movementComponent.Move(moveInput.x);
            
            // If movement input stops, return to idle state
            if (Mathf.Abs(moveInput.x) < 0.1f)
            {
                stateMachine.ProcessEvent(PlayerEvent.Idle);
            }
        }
    }

    private void HandleAttackPressed()
    {
        stateMachine.ProcessEvent(PlayerEvent.Attack);
    }
    
    private void HandleJumpPressed()
    {
        stateMachine.ProcessEvent(PlayerEvent.Jump);
    }
    
    public override void Exit()
    {
        base.Exit();
        
        if (owner.inputComponent != null)
        {
            owner.inputComponent.OnFirePerformed -= HandleAttackPressed;
            owner.inputComponent.OnJumpPerformed -= HandleJumpPressed;
        }
    }
}