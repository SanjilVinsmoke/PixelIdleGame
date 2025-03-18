

using UnityEngine;
using Utils;

    [StateDescription("Player is hit")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Yellow)]
    public class PlayerHitState: BaseState<Player, PlayerEvent>
    {
        private float hitStartTime;
        private float hitStunDuration = 1f;
        public override void Enter()
        {
            base.Enter();
            
            Hit();
            
        }
        
        private void Hit()
        {
            if (owner.healthComponent != null)
            {
                DOTweenHelper.Shake(owner.transform);
             
            }
            
        }
        
        public override void Update()
        {
            base.Update();
        
            // Check if hit stun duration has passed
            if (Time.time >= hitStartTime + hitStunDuration)
            {
                // Return to idle state
                stateMachine.ProcessEvent(PlayerEvent.Idle);
            }
        }
    }
