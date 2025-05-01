using System;
using UnityEngine;
using UnityEngine.Events;

namespace Component
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class SmashComponent : MonoBehaviour
    {
        [Header("Common Settings")]
        [SerializeField] private LayerMask damageableLayer;
        [SerializeField] private int smashDamage = 20;
        [SerializeField] private float smashCooldown = 1.5f;
        [SerializeField] private ParticleSystem smashEffectPrefab;
        
        [Header("Dash Smash Settings")]
        [SerializeField] private float dashSmashDistance = 5f;
        [SerializeField] private float dashSmashSpeed = 15f;
        [SerializeField] private float dashSmashDuration = 0.5f;
        [SerializeField] private AnimationCurve dashSmashSpeedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private Vector2 dashSmashHitboxSize = new Vector2(1.5f, 1f);
        
        [Header("Down Smash Settings")]
        [SerializeField] private float downSmashForce = 20f;
        [SerializeField] private float downSmashRadius = 3f;
        [SerializeField] private float downSmashImpactDelay = 0.3f;
        [SerializeField] private LayerMask groundLayer;
        
        // Events
        public UnityEvent onSmashStart;
        public UnityEvent onSmashImpact;
        public UnityEvent onSmashEnd;
        public UnityEvent<int> onSmashHit; // Passes damage amount
        
        // State tracking
        private float lastSmashTime = -Mathf.Infinity;
        private bool isDashing = false;
        private bool isSmashingDown = false;
        private Vector2 dashDirection;
        private float dashStartTime;
        private Vector3 originalPosition;
        private Rigidbody2D rb;
        private Collider2D coll;
        
        // Components
        private Animator animator;
        
        // Properties
        public bool CanSmash => Time.time >= lastSmashTime + smashCooldown;
        public bool IsDashing => isDashing;
        public bool IsSmashingDown => isSmashingDown;
        public bool IsSmashing => isDashing || isSmashingDown;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            animator = GetComponent<Animator>();
            
            // Initialize events
            if (onSmashStart == null) onSmashStart = new UnityEvent();
            if (onSmashImpact == null) onSmashImpact = new UnityEvent();
            if (onSmashEnd == null) onSmashEnd = new UnityEvent();
            if (onSmashHit == null) onSmashHit = new UnityEvent<int>();
        }
        
        private void Update()
        {
            // Handle Dash Smash movement
            if (isDashing)
            {
                float elapsed = Time.time - dashStartTime;
                float progress = elapsed / dashSmashDuration;
                
                if (progress >= 1f)
                {
                    CompleteDashSmash();
                }
                else
                {
                    float speedFactor = dashSmashSpeedCurve.Evaluate(progress);
                    Vector2 movement = dashDirection * dashSmashSpeed * speedFactor * Time.deltaTime;
                    transform.Translate(movement);
                    
                    // Check for hits during dash
                    CheckDashHits();
                }
            }
            
            // Handle Down Smash
            if (isSmashingDown && rb != null)
            {
                // Check for ground collision
                if (IsGrounded())
                {
                    PerformDownSmashImpact();
                }
            }
        }
        
        #region Public Methods
        
        /// <summary>
        /// Performs a dash smash in the specified direction
        /// </summary>
        public void DashSmash(Vector2 direction)
        {
            if (!CanSmash || IsSmashing)
                return;
                
            // Normalize direction
            if (direction.magnitude < 0.1f)
                direction = transform.right; // Default to facing direction
            else
                direction.Normalize();
                
            // Set up dash
            isDashing = true;
            dashDirection = direction;
            dashStartTime = Time.time;
            lastSmashTime = Time.time;
            originalPosition = transform.position;
            
            // Animation trigger
            if (animator != null)
                animator.SetTrigger("DashSmash");
                
            // Invoke start event
            onSmashStart?.Invoke();
            
            // Temporarily make character ignore certain collisions during dash
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
            
            Debug.Log($"Dash Smash started in direction {direction}");
        }
        
        /// <summary>
        /// Performs a downward smash
        /// </summary>
        public void DownSmash()
        {
            if (!CanSmash || IsSmashing || rb == null)
                return;
                
            // Set up down smash
            isSmashingDown = true;
            lastSmashTime = Time.time;
            
            // Apply downward force
            rb.linearVelocity = new Vector2(0, -downSmashForce);
            
            // Animation trigger
            if (animator != null)
                animator.SetTrigger("DownSmash");
                
            // Invoke start event
            onSmashStart?.Invoke();
            
            Debug.Log("Down Smash started");
        }
        
        /// <summary>
        /// Cancel any ongoing smash
        /// </summary>
        public void CancelSmash()
        {
            if (isDashing)
                CompleteDashSmash();
                
            if (isSmashingDown)
                CompleteDownSmash();
        }
        
        #endregion
        
        #region Private Methods
        
        private void CheckDashHits()
        {
            // Create a hitbox in front of the player in the dash direction
            Vector2 hitboxCenter = (Vector2)transform.position + dashDirection * (coll.bounds.extents.x + dashSmashHitboxSize.x / 2);
            
            // Check for hits
            Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter, dashSmashHitboxSize, 
                Vector2.SignedAngle(Vector2.right, dashDirection), damageableLayer);
                
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;
                    
                Interfaces.IDamageable damageable = hit.GetComponent<Interfaces.IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(smashDamage);
                    damageable.OnHit();
                    
                    // Invoke hit event
                    onSmashHit?.Invoke(smashDamage);
                    
                    // VFX at hit point
                    SpawnImpactEffect(hit.ClosestPoint(transform.position));
                    
                    Debug.Log($"Dash Smash hit {hit.gameObject.name} for {smashDamage} damage!");
                }
            }
        }
        
        private void CompleteDashSmash()
        {
            if (!isDashing)
                return;
                
            isDashing = false;
            
            // Re-enable collisions
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
            
            // End animation
            if (animator != null)
                animator.SetTrigger("SmashEnd");
                
            // Invoke end event
            onSmashEnd?.Invoke();
            
            Debug.Log("Dash Smash completed");
        }
        
        private void PerformDownSmashImpact()
        {
            // Already handled impact
            if (!isSmashingDown)
                return;
                
            // Impact effect
            SpawnImpactEffect(transform.position);
            
            // Detect and damage nearby enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, downSmashRadius, damageableLayer);
            
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;
                    
                Interfaces.IDamageable damageable = hit.GetComponent<Interfaces.IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(smashDamage);
                    damageable.OnHit();
                    
                    // Calculate knockback direction
                    Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                    
                    // Apply knockback if the object has a rigidbody
                    Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
                    if (hitRb != null)
                    {
                        hitRb.AddForce(knockbackDir * downSmashForce * 2f, ForceMode2D.Impulse);
                    }
                    
                    // Invoke hit event
                    onSmashHit?.Invoke(smashDamage);
                    
                    Debug.Log($"Down Smash hit {hit.gameObject.name} for {smashDamage} damage!");
                }
            }
            
            // Camera shake or other effects
            DOTweenHelper.DoCameraShake2D(Camera.main, 0.5f, 0.3f, 10);
            
            // Invoke impact event
            onSmashImpact?.Invoke();
            
            // Complete after a short delay
            Invoke(nameof(CompleteDownSmash), 0.2f);
        }
        
        private void CompleteDownSmash()
        {
            if (!isSmashingDown)
                return;
                
            isSmashingDown = false;
            
            // End animation
            if (animator != null)
                animator.SetTrigger("SmashEnd");
                
            // Invoke end event
            onSmashEnd?.Invoke();
            
            Debug.Log("Down Smash completed");
        }
        
        private bool IsGrounded()
        {
            if (coll == null) return false;
            
            // Check if we're touching the ground
            float extraHeight = 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(
                coll.bounds.center,
                Vector2.down, 
                coll.bounds.extents.y + extraHeight,
                groundLayer);
                
            return hit.collider != null;
        }
        
        private void SpawnImpactEffect(Vector3 position)
        {
            if (smashEffectPrefab != null)
            {
                ParticleSystem effect = Instantiate(smashEffectPrefab, position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration + 0.5f);
            }
        }
        
        // Visualize attack ranges in editor
        private void OnDrawGizmosSelected()
        {
            // Dash smash hitbox
            Gizmos.color = Color.red;
            Vector2 hitboxCenter = (Vector2)transform.position + Vector2.right * (GetComponent<Collider2D>()?.bounds.extents.x ?? 0.5f + dashSmashHitboxSize.x / 2);
            Gizmos.DrawWireCube(hitboxCenter, dashSmashHitboxSize);
            
            // Down smash radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, downSmashRadius);
        }
        
        #endregion
    }
}