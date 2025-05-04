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
        private bool isFacingWall;
        private bool isGrounded;
      
        
        // Check if enemy is facing wall 
        

        public override void Enter()
        {
            rb = owner.movementComponent.rb;
            base.Enter();
            owner.animationComponent.PlayAnimation(AnimationName.EnemyAnimationNames.RUN);
            
         
        }
        
        public override void Update()
        {
            base.Update();
            // Check if enemy is facing wall
           
            isFacingWall = PhysicsUtils.IsFacingWall(rb, owner.wallLayer ,2f, true);
            isGrounded = PhysicsUtils.IsGrounded(rb, owner.groundLayer ,2f , true);

            // Determine movement direction based on environment conditions
            float movementDirection = owner.moveSpeed;
            Debug.Log("Is facing wall: " + isFacingWall );
            Debug.Log("Is grounded: " + isGrounded );
            // If facing a wall, reverse direction
            if (isFacingWall)
            {
                movementDirection = -movementDirection;
            }
            
            // If not grounded, also reverse direction
            if (!isGrounded)
            {
                movementDirection = -movementDirection;
            }
            
            // Apply the final movement
            owner.movementComponent.Move(movementDirection);
        }
    }
     
}