using UnityEngine;
using Component.Interfaces;

namespace Component
{
    /// <summary>
    /// Handles horizontal movement for characters.
    /// Integrates with IWallCheck to prevent pushing into walls.
    /// </summary>
    public class MoveComponent : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float speed = 0f;
        
        [Header("References")]
        public Rigidbody2D rb;

        private bool isFacingRight = true;
        private IWallCheck wallCheck;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            wallCheck = GetComponent<WallCheckComponent>();
            
            if (rb == null)
            {
                Debug.LogError($"[MoveComponent] No Rigidbody2D found on {gameObject.name}!");
            }
            
            // WallCheck is optional, but recommended
            if (wallCheck == null)
            {
                Debug.LogWarning($"[MoveComponent] No WallCheckComponent found on {gameObject.name}. Wall detection disabled.");
            }
        }

        /// <summary>
        /// Applies horizontal movement based on input (-1 to 1).
        /// If input is zero, stops velocity immediately to prevent sliding.
        /// Prevents movement into walls if WallCheckComponent is present.
        /// </summary>
        public void Move(float horizontalInput)
        {
            if (rb == null) return;
            
            // Instant stop when no input
            if (Mathf.Approximately(horizontalInput, 0f))
            {
                Vector2 vel = rb.linearVelocity;
                vel.x = 0f;
                rb.linearVelocity = vel;
                return;
            }

            // Check for wall blocking movement (if WallCheckComponent exists)
            if (wallCheck != null && wallCheck.IsWallInDirection(horizontalInput))
            {
                // Stop horizontal movement when hitting a wall
                // Preserve vertical velocity for jumping/falling
                Vector2 vel = rb.linearVelocity;
                vel.x = 0f;
                rb.linearVelocity = vel;
                return;
            }

            // Regular movement
            Vector2 velocity = rb.linearVelocity;
            velocity.x = horizontalInput * speed;
            rb.linearVelocity = velocity;

            HandleFlip(horizontalInput);
        }

        /// <summary>
        /// Applies a friction factor (0 to 1) to horizontal velocity.
        /// Optionally, use PhysicsMaterial2D or Rigidbody2D.drag instead.
        /// </summary>
        public void ApplyFriction(float friction)
        {
            if (rb == null) return;
            
            Vector2 vel = rb.linearVelocity;
            vel.x *= (1f - friction);
            rb.linearVelocity = vel;
        }

        /// <summary>
        /// Handles sprite flipping based on movement direction.
        /// </summary>
        /// <param name="horizontalInput">Movement input direction</param>
        private void HandleFlip(float horizontalInput)
        {
            if (horizontalInput > 0 && !isFacingRight)
                Flip();
            else if (horizontalInput < 0 && isFacingRight)
                Flip();
        }

        /// <summary>
        /// Flips the character sprite horizontally.
        /// </summary>
        public void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }
}