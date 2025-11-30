using Constant;
using Managers;
using UnityEngine;
using Utils;

namespace StateMachine
{
    /// <summary>
    /// State representing the enemy death behavior.
    /// Handles death animation, disables components, and manages enemy cleanup.
    /// </summary>
    [StateDescription("Enemy has died")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Gray)]
    public class EnemyDeathState<T> : BaseState<T, EnemyEvent> where T : Enemy<T>
    {
        private float deathTimer;
        private const float DeathDelay = 2f; // Time to wait before destroying the enemy
        private bool componentsDisabled;

        /// <summary>
        /// Called when entering the death state.
        /// Plays death animation and begins cleanup process.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            if (owner?.animationComponent != null)
            {
                owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.DEATH);
            }
            
            deathTimer = 0f;
            componentsDisabled = false;
            
            // Disable movement and attack components
            DisableComponents();
        }

        /// <summary>
        /// Called each frame while in death state.
        /// Manages death timer and enemy destruction.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            deathTimer += Time.deltaTime;
            
            // Destroy enemy after death animation completes
            if (deathTimer >= DeathDelay)
            {
                DestroyEnemy();
            }
        }

        /// <summary>
        /// Disables enemy components to prevent further actions during death.
        /// </summary>
        private void DisableComponents()
        {
            if (componentsDisabled || owner == null) return;
            
            // Disable movement
            if (owner.movementComponent != null)
            {
                owner.movementComponent.enabled = false;
            }
            
            // Disable attack
            if (owner.attackComponent != null)
            {
                owner.attackComponent.enabled = false;
            }
            
            // Disable damage dealer
            if (owner.damageableComponent != null)
            {
                owner.damageableComponent.enabled = false;
            }
            
            // Stop rigidbody movement
            if (owner.movementComponent?.rb != null)
            {
                owner.movementComponent.rb.linearVelocity = Vector2.zero;
                owner.movementComponent.rb.isKinematic = true;
            }
            
            componentsDisabled = true;
        }

        /// <summary>
        /// Destroys the enemy game object.
        /// Can be extended to include loot drops, score updates, etc.
        /// </summary>
        private void DestroyEnemy()
        {
            if (owner != null)
            {
                // TODO: Add loot drop logic here
                // TODO: Add score/experience points here
                // TODO: Add death VFX/SFX here
                
                Object.Destroy(owner.gameObject);
            }
        }

        /// <summary>
        /// Called when exiting the death state.
        /// Note: This typically won't be called as the enemy is destroyed.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
        }
    }
}