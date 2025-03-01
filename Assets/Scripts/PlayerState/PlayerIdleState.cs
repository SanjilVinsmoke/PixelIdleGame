using UnityEngine;
using Utils;

[StateDescription("Player is idling")]
[StateDebugColor(StateDebugColorAttribute.UnityColor.Green)]
public class PlayerIdleState : BaseState<Player, PlayerEvent>
{
    public override void Enter()
    {
        base.Enter();
        
        DOTweenHelper.DoPulse(owner.transform, 1.2f, 1.2f);
        
        // Play the idle animation if an animator exists
        // if (owner.animatorComponent != null)
        // {
        //     owner.animatorComponent.Play("Idle");
        // }
    }

    public override void Update()
    {
        base.Update();

        // Check for input to attack.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ProcessEvent(PlayerEvent.Attack); // Use event instead of direct state change
        }

        // // Check if the player's health is depleted.
        // if (owner.healthComponent != null && owner.healthComponent.IsDead)
        // {
        //     stateMachine.ProcessEvent(PlayerEvent.Die);
        // }
    }
}