using Constant;
using UnityEngine;
using Utils;

[StateDescription("Player is idling")]
[StateDebugColor(StateDebugColorAttribute.UnityColor.Green)]
public class PlayerIdleState : BaseState<Player, PlayerEvent>
{
    public override void Enter()
    {
        base.Enter();
      
        owner.OnAttackButtonPressed += HandleAttackPressed;
        owner.OnJumpButtonPressed += HandleJumpPressed;
       
        
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
    public override void Exit()
    {
        base.Exit();
     
        owner.OnAttackButtonPressed -= HandleAttackPressed;
    }
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