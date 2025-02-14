using Test;
using UnityEngine;
using Utils;
[StateDescription("Player is idling")]
[StateDebugColor(StateDebugColorAttribute.UnityColor.Green)]
public class PlayerIdleState :BaseState<Player>
{
    public override void Enter()
    {
        base.Enter();
       
    }

    public override void Update()
    {
        base.Update();
      
    }
}

