using Constant;
using Managers;
using UnityEngine;
using Utils;

namespace StateMachine
{
    /// <summary>
    /// State representing the enemy roaming behavior.
    /// The enemy patrols back and forth, turning around when detecting walls or ledges.
    /// </summary>
    [StateDescription("Enemy is roaming between waypoints")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Green)]
    public class EnemyRoamState<T> : BaseState<T, EnemyEvent> where T : Enemy<T>
    {
        private Rigidbody2D rb;
        private bool isGroundAhead;
        private bool isWallAhead;
        
        private float dir = 1f; // Current facing direction: +1 right, -1 left
        private const float RayOriginYOffset = -0.5f; // Adjust to character's feet
        private const float CheckDistance = 0.2f; // Distance to check ahead for obstacles
        
        /// <summary>
        /// Called when entering the roam state.
        /// Caches rigidbody reference and starts run animation.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            if (owner?.movementComponent != null)
            {
                rb = owner.movementComponent.rb;
            }
            
            if (owner?.animationComponent != null)
            {
                owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.RUN);
            }
        }
        
        /// <summary>
        /// Called each frame while in roam state.
        /// Checks for obstacles and ledges, turns around if needed, and moves the enemy.
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            if (rb == null || owner?.enemyData == null) return;
            
            Vector2 forward = Vector2.right * dir;
            
            // Check for ground ahead to prevent falling off ledges
            isGroundAhead = PhysicsUtils.IsGroundAhead(
                rb,
                owner.enemyData.groundLayer,
                RayOriginYOffset,
                forward,
                CheckDistance,
                debugMode: true
            );
            
            // Check for walls ahead
            isWallAhead = PhysicsUtils.IsFacingWall(
                rb, 
                owner.enemyData.wallLayer, 
                dir, 
                1f, 
                debugMode: true
            );
            
            Debug.Log($"isGroundAhead: {isGroundAhead}, isWallAhead: {isWallAhead}");
            
            // Turn around if no ground ahead or if facing a wall
            if (!isGroundAhead || isWallAhead)
            {
                dir *= -1f;
            }
            
            // Move the enemy
            if (owner?.movementComponent != null)
            {
                owner.movementComponent.Move(dir);
            }
        }
        
        /// <summary>
        /// Called when exiting the roam state.
        /// Stops enemy movement.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            
            // Stop movement when exiting roam state
            if (owner?.movementComponent?.rb != null)
            {
                owner.movementComponent.rb.linearVelocity = Vector2.zero;
            }
        }
    }
}