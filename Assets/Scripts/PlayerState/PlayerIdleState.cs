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
        DOTweenHelper.DoPulse(owner.transform, 1.2f, 1.2f);
        
        // Play the idle animation if an animator exists
        // if (owner.animatorComponent != null)
        // {
        //     owner.animatorComponent.Play("Idle");
        // }
    }

    private void HandleAttackPressed()
    {
        stateMachine.ProcessEvent(PlayerEvent.Attack);
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