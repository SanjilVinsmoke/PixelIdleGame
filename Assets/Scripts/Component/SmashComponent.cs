using System;
using UnityEngine;
using UnityEngine.Events;

namespace Component
{
    [RequireComponent(typeof(Rigidbody2D))]
 
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
        public UnityEvent<int> onSmashHit;

        // State tracking
        private float lastSmashTime = -Mathf.Infinity;
        private bool isDashing = false;
        private bool isSmashingDown = false;
        private Vector2 dashDirection;
        private float dashStartTime;

        // Components
        private Rigidbody2D rb;
        [SerializeField] private Collider2D coll;

        // Public properties
        public bool CanSmash => Time.time >= lastSmashTime + smashCooldown;
        public bool IsDashing => isDashing;
        public bool IsSmashingDown => isSmashingDown;
        public bool IsSmashing => isDashing || isSmashingDown;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
          

            // Initialize events
            if (onSmashStart == null) onSmashStart = new UnityEvent();
            if (onSmashImpact == null) onSmashImpact = new UnityEvent();
            if (onSmashEnd == null) onSmashEnd = new UnityEvent();
            if (onSmashHit == null) onSmashHit = new UnityEvent<int>();
        }

        private void Update()
        {
            if (isDashing)
            {
                HandleDashMovement();
            }

            if (isSmashingDown && rb != null)
            {
                    PerformDownSmashImpact();
                
            }
        }

        #region Public Methods

        public void DashSmash(Vector2 direction)
        {
            if (!CanSmash || IsSmashing) return;
            direction = direction.magnitude < 0.1f ? Vector2.right : direction.normalized;

            isDashing = true;
            dashDirection = direction;
            dashStartTime = Time.time;
            lastSmashTime = Time.time;

            onSmashStart?.Invoke();
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
        }

        public void DownSmash()
        {
            if (!CanSmash || IsSmashing || rb == null) return;

            isSmashingDown = true;
            lastSmashTime = Time.time;

            rb.linearVelocity = new Vector2(0, -downSmashForce);
            onSmashStart?.Invoke();
        }

        public void CancelSmash()
        {
            if (isDashing) CompleteDashSmash();
            if (isSmashingDown) CompleteDownSmash();
        }

        #endregion

        #region Private Methods

        private void HandleDashMovement()
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
                CheckDashHits();
            }
        }

        private void CheckDashHits()
        {
            Vector2 hitboxCenter = (Vector2)transform.position + dashDirection * (coll.bounds.extents.x + dashSmashHitboxSize.x / 2);
            Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCenter, dashSmashHitboxSize,
                Vector2.SignedAngle(Vector2.right, dashDirection), damageableLayer);

            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;
                var damageable = hit.GetComponent<Interfaces.IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(smashDamage);
                    damageable.OnHit();
                    onSmashHit?.Invoke(smashDamage);
                    SpawnImpactEffect(hit.ClosestPoint(transform.position));
                }
            }
        }

        private void CompleteDashSmash()
        {
            if (!isDashing) return;
            isDashing = false;
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
            onSmashEnd?.Invoke();
        }

        private void PerformDownSmashImpact()
        {
            Debug.DrawRay(transform.position, Vector2.down * downSmashRadius, Color.red);
            if (!isSmashingDown) return;

            SpawnImpactEffect(transform.position);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, downSmashRadius, damageableLayer);
            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;
                var damageable = hit.GetComponent<Interfaces.IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(smashDamage);
                    damageable.OnHit();
                    var knockbackDir = (hit.transform.position - transform.position).normalized;
                    var hitRb = hit.GetComponent<Rigidbody2D>();
                    if (hitRb != null)
                        hitRb.AddForce(knockbackDir * downSmashForce * 2f, ForceMode2D.Impulse);
                    onSmashHit?.Invoke(smashDamage);
                }
            }

            onSmashImpact?.Invoke();
            Invoke(nameof(CompleteDownSmash), downSmashImpactDelay);
        }

        private void CompleteDownSmash()
        {
            if (!isSmashingDown) return;
            isSmashingDown = false;
            onSmashEnd?.Invoke();
           
        }

        private bool IsGrounded()
        {
            float extraHeight = 0.1f;
            var hit = Physics2D.Raycast(coll.bounds.center, Vector2.down,
                coll.bounds.extents.y + extraHeight, groundLayer);
            return hit.collider != null;
        }

        private void SpawnImpactEffect(Vector3 position)
        {
            if (smashEffectPrefab != null)
            {
                var effect = Instantiate(smashEffectPrefab, position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration + 0.5f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector2 hitboxCenter = (Vector2)transform.position + Vector2.right *
                (coll?.bounds.extents.x ?? 0.5f + dashSmashHitboxSize.x / 2);
            Gizmos.DrawWireCube(hitboxCenter, dashSmashHitboxSize);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, downSmashRadius);
        }
        #endregion
    }
}