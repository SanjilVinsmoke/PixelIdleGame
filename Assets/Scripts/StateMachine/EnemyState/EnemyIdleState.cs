using Constant;
using Managers;
using UnityEngine;
using Utils;

namespace StateMachine
{
    [StateDescription("Enemy is idle, watching the player")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Yellow)]
    public class EnemyIdleState<T>: BaseState<T, EnemyEvent> where T : Enemy<T>
    {
        
        private float idleTimer = 0f;
        private const float maxIdleTime = 2f; // Maximum time to stay idle if player is still detected
        
        public override void Enter()
        {
            base.Enter();
            owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.IDLE); // Don't loop idle animation
            idleTimer = 0f;
        }
        
        public override void Update()
        {
            base.Update();
            
            // If we've been idle for too long while seeing the player, consider attacking
            idleTimer += Time.deltaTime;
            if (idleTimer >= maxIdleTime)
            {
                // For now, just reset the timer
                // In the future, you might want to transition to an attack state
                idleTimer = 0f;
            }
        }
        
        public override void Exit()
        {
            base.Exit();
        }
    }
}