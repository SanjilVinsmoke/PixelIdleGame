using UnityEngine;
using Component.Interfaces;
using ScriptableObjects;

namespace Component
{
    /// <summary>
    /// Detects walls on either side of the character.
    /// Provides information about wall contact for other components to use.
    /// Uses simple raycasts similar to enemy wall detection.
    /// </summary>
    
    public class WallCheckComponent : MonoBehaviour, IWallCheck
    {
        [Header("Wall Detection Settings")]
        [Tooltip("Distance to check for walls")]
        [SerializeField] private float wallCheckDistance = 0.5f;
        
        [Tooltip("Layers that count as walls (NOT the player layer!)")]
        [SerializeField] private LayerMask wallLayer;

        [Header("Debug (Runtime Only)")]
        [Tooltip("Show debug rays during play mode (separate from Gizmo Settings)")]
        [SerializeField] private bool showDebugRays = false;
        [SerializeField] private bool logDetections = false;

        // Cached references
        private Rigidbody2D rb;
       
        
        // Public properties
        public bool IsTouchingWallRight { get; private set; }
        public bool IsTouchingWallLeft { get; private set; }
        public bool IsTouchingAnyWall => IsTouchingWallRight || IsTouchingWallLeft;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (rb == null)
            {
                Debug.LogError($"[WallCheckComponent] No Rigidbody2D found on {gameObject.name}!");
            }
        }

        private void FixedUpdate()
        {
            CheckWalls();
        }

        /// <summary>
        /// Performs raycasts to detect walls on both sides.
        /// Uses simple raycast approach similar to enemy wall detection.
        /// </summary>
        private void CheckWalls()
        {
            if (rb == null)
            {
                IsTouchingWallRight = false;
                IsTouchingWallLeft = false;
                return;
            }
            
            // Check walls using raycasts (similar to enemy implementation)
            IsTouchingWallRight = CheckWallInDirection(Vector2.right);
            IsTouchingWallLeft = CheckWallInDirection(Vector2.left);
        }

        /// <summary>
        /// Checks for a wall in a specific direction using raycast.
        /// Simple raycast approach like PhysicsUtils.IsFacingWall()
        /// </summary>
        /// <param name="direction">Direction to check (Vector2.left or Vector2.right)</param>
        /// <returns>True if wall detected, false otherwise</returns>
        private bool CheckWallInDirection(Vector2 direction)
        {
            // Cast from rigidbody position (similar to enemy wall check)
            Vector2 origin = rb.position;
            
            // Perform raycast
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, wallLayer);
            
            // Debug visualization
            if (showDebugRays)
            {
                Color rayColor = hit.collider != null ? Color.red : Color.green;
                Debug.DrawRay(origin, direction * wallCheckDistance, rayColor);
            }
            
            // Log detection if enabled
            if (hit.collider != null && logDetections)
            {
                Debug.Log($"[WallCheck] Detected wall: {hit.collider.gameObject.name} " +
                          $"in direction {direction} at distance {hit.distance}");
            }
            
            // Return true if we hit something
            return hit.collider != null;
        }

        /// <summary>
        /// Checks if there's a wall in the direction the character is trying to move.
        /// </summary>
        /// <param name="moveDirection">Horizontal movement direction (-1 for left, +1 for right)</param>
        /// <returns>True if wall in that direction, false otherwise</returns>
        public bool IsWallInDirection(float moveDirection)
        {
            if (moveDirection > 0.01f)
                return IsTouchingWallRight;
            else if (moveDirection < -0.01f)
                return IsTouchingWallLeft;
            
            return false;
        }

        /// <summary>
        /// Visualizes wall detection rays in the scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            if (rb == null) return;
            
            var settings = GizmoSettingsSo.Instance;
            if (settings == null || !settings.ShouldDraw(GizmoCategory.WallCheck)) return;

            Vector2 origin = rb.position;

            // Draw detection rays
            Gizmos.color = IsTouchingWallRight ? settings.wallCheckActiveColor : settings.wallCheckInactiveColor;
            Gizmos.DrawLine(origin, origin + Vector2.right * wallCheckDistance);
            Gizmos.DrawWireSphere(origin + Vector2.right * wallCheckDistance, 0.05f);
            
            Gizmos.color = IsTouchingWallLeft ? settings.wallCheckActiveColor : settings.wallCheckInactiveColor;
            Gizmos.DrawLine(origin, origin + Vector2.left * wallCheckDistance);
            Gizmos.DrawWireSphere(origin + Vector2.left * wallCheckDistance, 0.05f);
            #endif
        }
    }
}

