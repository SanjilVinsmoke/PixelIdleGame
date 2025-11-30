using Constant;
using Managers;
using UnityEngine;
using Utils;

namespace StateMachine
{
    /// <summary>
    /// State representing the enemy idle behavior.
    /// The enemy is stationary and watching for player activity.
    /// </summary>
    [StateDescription("Enemy is idle, watching the player")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Yellow)]
    public class EnemyIdleState<T>: BaseState<T, EnemyEvent> where T : Enemy<T>
    {
        private float idleTimer = 0f;
        private const float MaxIdleTime = 2f; // Maximum time to stay idle if player is still detected
        
        /// <summary>
        /// Called when entering the idle state.
        /// Initializes idle animation and resets the idle timer.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            if (owner?.animationComponent != null)
            {
                owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.IDLE);
            }
            
            idleTimer = 0f;
        }
        
        /// <summary>
        /// Called each frame while in idle state.
        /// Tracks idle duration and can trigger transitions after max idle time.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            // If we've been idle for too long while seeing the player, consider attacking
            idleTimer += Time.deltaTime;
            if (idleTimer >= MaxIdleTime)
            {
                // For now, just reset the timer
                // In the future, you might want to transition to an attack state
                idleTimer = 0f;
            }
        }
        
        /// <summary>
        /// Called when exiting the idle state.
        /// Performs any necessary cleanup.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            idleTimer = 0f;
        }
    }
}