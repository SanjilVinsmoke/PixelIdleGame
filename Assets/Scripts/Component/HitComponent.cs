using UnityEngine;
using System.Collections;
using Component.Interfaces;

namespace Component
{
    public class HitComponent : MonoBehaviour
    {
        [Header("Hit Reaction Settings")]
        [SerializeField] private float knockbackForce = 5f;
        [SerializeField] private float knockbackDuration = 0.2f;
        [SerializeField] private float invincibilityDuration = 1.0f; // Time player is invincible after being hit

        private Rigidbody2D rb;
        private HealthComponent healthComponent; // To check if alive and potentially trigger state changes
        private bool isHit = false; // Tracks if currently in knockback/hit state
        private bool isInvincible = false;
        private Coroutine knockbackCoroutine;
        private Coroutine invincibilityCoroutine;

        public bool IsHit => isHit;
        public bool IsInvincible => isInvincible;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            healthComponent = GetComponent<HealthComponent>(); // Assuming HealthComponent is on the same GameObject
            if (rb == null)
                Debug.LogError($"[HitComponent] No Rigidbody2D found on {gameObject.name}");
            // HealthComponent is optional for basic hit reaction
            // if (healthComponent == null)
            //     Debug.LogWarning($"[HitComponent] No HealthComponent found on {gameObject.name}. Hit reaction might not integrate fully.");
        }

        // Called when the player takes damage (e.g., from Player.TakeDamage)
        public void TakeHit(Vector2 hitDirection)
        {
            if (isInvincible || rb == null) // Don't react if invincible or no Rigidbody
            {
                return;
            }

            // Cancel previous knockback if any
            if (knockbackCoroutine != null)
            {
                StopCoroutine(knockbackCoroutine);
            }
            // Cancel previous invincibility if any (shouldn't happen often, but good practice)
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            isHit = true; // Enter hit state
            isInvincible = true;

            knockbackCoroutine = StartCoroutine(KnockbackCoroutine(hitDirection));
            invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
        }

        private IEnumerator KnockbackCoroutine(Vector2 hitDirection)
        {
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f; // Optional: Modify gravity during knockback
            rb.linearVelocity = Vector2.zero; // Stop current movement

            // Apply knockback force - Ensure direction is normalized if needed
            // If hitDirection is zero, maybe apply knockback upwards or based on facing direction?
            Vector2 knockbackVelocity = (hitDirection == Vector2.zero ? Vector2.up : hitDirection.normalized) * knockbackForce;
            rb.AddForce(knockbackVelocity, ForceMode2D.Impulse);

            yield return new WaitForSeconds(knockbackDuration);

            rb.gravityScale = originalGravity; // Restore gravity
            rb.linearVelocity = Vector2.zero; // Stop knockback movement
            isHit = false; // Exit hit state (knockback finished)
            knockbackCoroutine = null;
        }

        private IEnumerator InvincibilityCoroutine()
        {
            // Optional: Add visual feedback for invincibility (e.g., flashing sprite)
            yield return new WaitForSeconds(invincibilityDuration);
            isInvincible = false;
            // Optional: Remove visual feedback
            invincibilityCoroutine = null;
        }

        // Optional: Method to cancel hit effects prematurely (e.g., on death)
        public void CancelHitEffects()
        {
            if (knockbackCoroutine != null)
            {
                StopCoroutine(knockbackCoroutine);
               // rb.gravityScale = GetComponent<MoveComponent>()?.OriginalGravityScale ?? 1f; // Restore gravity (assuming MoveComponent holds original)
                rb.linearVelocity = Vector2.zero;
                isHit = false;
                knockbackCoroutine = null;
            }
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
                isInvincible = false;
                invincibilityCoroutine = null;
                // Optional: Remove visual feedback
            }
        }
    }
}