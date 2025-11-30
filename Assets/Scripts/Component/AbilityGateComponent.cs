using UnityEngine;
using Managers;

namespace Component
{
    /// <summary>
    /// A physical barrier that only opens/disables when the player has a specific ability.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AbilityGateComponent : MonoBehaviour
    {
        [Header("Gate Settings")]
        [Tooltip("The ability required to pass this gate.")]
        [SerializeField] private PlayerAbility requiredAbility;

        [Tooltip("If true, the game object is destroyed when unlocked. If false, only the collider is disabled.")]
        [SerializeField] private bool destroyOnUnlock = false;

        [Header("Visuals")]
        [Tooltip("Optional sprite to show when locked.")]
        [SerializeField] private Sprite lockedSprite;
        [Tooltip("Optional sprite to show when unlocked.")]
        [SerializeField] private Sprite unlockedSprite;
        
        private Collider2D gateCollider;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            gateCollider = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            CheckAbility();
            
            // Subscribe to future unlocks
            if (PlayerAbilityManager.Instance != null)
            {
                PlayerAbilityManager.Instance.OnAbilityUnlocked += HandleAbilityUnlocked;
            }
        }

        private void OnDestroy()
        {
            if (PlayerAbilityManager.Instance != null)
            {
                PlayerAbilityManager.Instance.OnAbilityUnlocked -= HandleAbilityUnlocked;
            }
        }

        private void HandleAbilityUnlocked(PlayerAbility ability)
        {
            if (ability == requiredAbility)
            {
                UnlockGate();
            }
        }

        private void CheckAbility()
        {
            if (PlayerAbilityManager.Instance != null && PlayerAbilityManager.Instance.HasAbility(requiredAbility))
            {
                UnlockGate();
            }
            else
            {
                LockGate();
            }
        }

        private void UnlockGate()
        {
            Debug.Log($"Opening Gate: Required Ability {requiredAbility} Acquired.");

            if (destroyOnUnlock)
            {
                Destroy(gameObject);
            }
            else
            {
                if (gateCollider != null) gateCollider.enabled = false;
                if (spriteRenderer != null && unlockedSprite != null)
                {
                    spriteRenderer.sprite = unlockedSprite;
                }
            }
        }

        private void LockGate()
        {
            if (gateCollider != null) gateCollider.enabled = true;
            if (spriteRenderer != null && lockedSprite != null)
            {
                spriteRenderer.sprite = lockedSprite;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                 if (PlayerAbilityManager.Instance != null && !PlayerAbilityManager.Instance.HasAbility(requiredAbility))
                 {
                     Debug.Log($"Gate Locked. Requires: {requiredAbility}");
                     // TODO: Add visual feedback or UI message here
                 }
            }
        }
    }
}

