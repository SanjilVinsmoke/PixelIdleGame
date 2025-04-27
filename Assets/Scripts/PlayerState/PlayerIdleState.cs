using System.Transactions;
using Constant;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

[StateDescription("Player is idling")]
[StateDebugColor(StateDebugColorAttribute.UnityColor.Green)]
public class PlayerIdleState : BaseState<Player, PlayerEvent>
{
    public override void Enter()
    {
        base.Enter();
        
        // Register event handlers from InputComponent
        if (owner.inputComponent != null)
        {
            owner.inputComponent.OnAttackPerformed += HandleAttackPressed;
            owner.inputComponent.OnJumpPerformed += HandleJumpPressed;
            owner.inputComponent.OnMovePerformed += HandleMovePressed;
        }
        
        // Play the idle animation if an animator exists
        if (owner.animationComponent != null)
        {
            owner.animationComponent.PlayAnimation(AnimationName.PlayerAnimationNames.IDLE);
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

    private void HandleMovePressed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.magnitude > 0.1f)
        {
            stateMachine.ProcessEvent(PlayerEvent.Move);
        }
    }

    public override void Exit()
    {
        base.Exit();
    
        if (owner.inputComponent != null)
        {
            owner.inputComponent.OnAttackPerformed -= HandleAttackPressed;
            owner.inputComponent.OnJumpPerformed -= HandleJumpPressed;
            owner.inputComponent.OnMovePerformed -= HandleMovePressed;
        }
    }
    
    // if Input detected, process the event
    
    
    public override void Update()
    {
        base.Update();

        // Check for input to attack.
       
    
       

        // // Check if the player's health is depleted.
        // if (owner.healthComponent != null && owner.healthComponent.IsDead)
        // {
        //     stateMachine.ProcessEvent(PlayerEvent.Die);
        // }
    }
}