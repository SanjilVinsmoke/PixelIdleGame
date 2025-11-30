using Constant;
using Managers;
using UnityEngine;
using Utils;

namespace StateMachine
{
    /// <summary>
    /// State representing the enemy attacking behavior.
    /// Handles attack animation, cooldown, and damage dealing.
    /// </summary>
    [StateDescription("Enemy is attacking the player")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Red)]
    public class EnemyAttackState<T> : BaseState<T, EnemyEvent> where T : Enemy<T>
    {
        private float attackCooldownTimer;

        /// <summary>
        /// Called when entering the attack state.
        /// Initializes attack animation and cooldown timer.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            if (owner?.animationComponent != null)
            {
                owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.ATTACK);
            }
            
            attackCooldownTimer = 0f;
            
            // Perform the attack
            PerformAttack();
        }

        /// <summary>
        /// Called each frame while in attack state.
        /// Manages attack cooldown and transitions back to idle.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            if (owner?.attackComponent == null) return;
            
            attackCooldownTimer += Time.deltaTime;
            
            // Return to idle after attack animation/cooldown
            if (attackCooldownTimer >= owner.attackComponent.attackCooldown)
            {
                stateMachine?.ProcessEvent(EnemyEvent.Idle);
            }
        }

        /// <summary>
        /// Executes the attack logic using the attack component.
        /// </summary>
        private void PerformAttack()
        {
            if (owner?.attackComponent != null)
            {
                owner.attackComponent.Attack();
            }
            else
            {
            
            
                Debug.LogWarning($"Attack component not found on {owner?.name}. Cannot perform attack.");
            }
        }

        /// <summary>
        /// Called when exiting the attack state.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            attackCooldownTimer = 0f;
        }
    }
}