using Constant;
using Managers;
using UnityEngine;
using Utils;

namespace StateMachine
{
    /// <summary>
    /// State representing the enemy being hit by an attack.
    /// Handles knockback effect, hit animation, and recovery.
    /// </summary>
    [StateDescription("Enemy is taking damage and being knocked back")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Orange)]
    public class EnemyHitState<T> : BaseState<T, EnemyEvent> where T : Enemy<T>
    {
        private float knockBackTimer;
        private Vector2 knockBackDirection;
        
        /// <summary>
        /// Called when entering the hit state.
        /// Plays hit animation and calculates knockback direction.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            if (owner?.animationComponent != null)
            {
                owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.HIT);
            }
            
            if (owner?.enemyData != null)
            {
                knockBackTimer = owner.enemyData.knockbackDuration;
            }
            else
            {
                knockBackTimer = 0.2f; // Default fallback
            }
            
            // Calculate knockback direction from attack position
            if (owner != null)
            {
                knockBackDirection = (owner.transform.position - owner.lastAttackPosition).normalized;
                ApplyKnockback();
            }
        }

        /// <summary>
        /// Called each frame while in hit state.
        /// Manages knockback duration and transition back to idle.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            knockBackTimer -= Time.deltaTime;
            
            // Return to idle state after knockback duration
            if (knockBackTimer <= 0f)
            {
                stateMachine?.ProcessEvent(EnemyEvent.Idle);
            }
        }

        /// <summary>
        /// Applies knockback force to the enemy's rigidbody.
        /// </summary>
        private void ApplyKnockback()
        {
            if (owner?.movementComponent?.rb == null || owner?.enemyData == null) return;
            
            float knockbackForce = owner.enemyData.knockbackForce;
            owner.movementComponent.rb.linearVelocity = knockBackDirection * knockbackForce;
        }

        /// <summary>
        /// Called when exiting the hit state.
        /// Resets velocity and prepares for next state.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            
            // Stop any remaining knockback velocity
            if (owner?.movementComponent?.rb != null)
            {
                owner.movementComponent.rb.linearVelocity = Vector2.zero;
            }
            
            knockBackTimer = 0f;
        }
    }
}