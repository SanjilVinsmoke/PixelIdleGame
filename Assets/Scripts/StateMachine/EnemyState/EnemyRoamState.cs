using Constant;
using Managers;
using NUnit.Framework;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace StateMachine
{
    [StateDescription("Enemy is roaming between waypoints")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Green)]
    public class EnemyRoamState<T> : BaseState<Worm , EnemyEvent> 
    {
        private Rigidbody2D rb;
  
        private bool isGroundAhead;
        private bool isWallAhead;
      
        private float          dir = .1f;               // current facing direction: +1 right, -1 left
        private const float    rayOriginYOffset = -0.5f; // adjust to character's feet
        private const float    checkDistance     = .2f;
        // Check if enemy is facing wall 
        

        public override void Enter()
        {
            rb = owner.movementComponent.rb;
            base.Enter();
            owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.RUN);
            dir =owner.enemyData.moveSpeed;
            

        }
        
        public override void Update()
        {
            base.Update();
           
            Vector2  origin  = rb.position + Vector2.up * rayOriginYOffset;
            Vector2  forward = Vector2.right * dir;
        
            isGroundAhead = PhysicsUtils.IsGroundAhead(
                rb,
                owner.enemyData.groundLayer,
                rayOriginYOffset,
                forward,
                checkDistance,
                debugMode: true
            );

           
            // Pass the direction to IsFacingWall
            isWallAhead = PhysicsUtils.IsFacingWall(rb, owner. enemyData.wallLayer, dir, 1f, debugMode: true);
            Debug.Log($"isGroundAhead: {isGroundAhead}, isWallAhead: {isWallAhead}");
            if (!isGroundAhead || isWallAhead)
            {
                dir *= -1f;
            }
            
            owner.movementComponent.Move(dir);

        }
    }
     
}