using UnityEngine;

namespace ScriptableObjects.PlayerScriptable
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data/Player Data", order = 0)]
    public class PlayerDataSo : ScriptableObject
    {
        [Header("Health Stats")]
        public float maxHealth = 100f;
        public float currentHealth; // Current health will likely be managed at runtime

        [Header("Movement Core")]
        public float moveSpeed = 5f;
        public float deadZone = 0.1f; // Input deadzone for movement and other actions

        [Header("Jumping")]
        public float jumpForce = 10f;
        public float gravityScale = 1f;
        public float groundCheckDistance = 0.1f;
        public LayerMask groundLayer;
        // Potentially: doubleJumpEnabled, coyoteTimeDuration, jumpBufferDuration

        [Header("Attacking")]
        public int attackDamage = 10;
        public float attackRange = 1f;
        public float attackCooldown = 0.5f; // Cooldown for basic attack
        public LayerMask damageableLayer;

        [Header("Dashing")]
        // DashComponent specific settings might be extensive.
        // Consider if a separate DashSO is needed or include key params here.
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;

        [Header("Rolling")]
        // RollComponent specific settings.
        public float rollSpeed = 8f;
        public float rollDuration = 0.5f;
        public float rollCooldown = 1f;
        public bool canInterruptRollWithJump = true;
        public bool canInterruptRollWithAttack = true;

        [Header("Smashing (Down Smash)")]
        // SmashComponent specific settings.
        public int smashDamage = 25;
        public float downSmashForce = 20f;
        // public float dashSmashSpeed = 15f; // Example if you have dash smash
        public float smashCooldown = 1.5f;

        [Header("Hit/Knockback")]
        // HitComponent specific settings
        public float knockbackForce = 5f;
        public float knockbackDuration = 0.3f;

        // Add any other player-specific data here
        // For example, experience, level, currency, animation names (though often better in an AnimationDataSO)

        /// <summary>
        /// Initializes runtime values from this ScriptableObject.
        /// Call this when the player is created or data is loaded.
        /// </summary>
        public void InitializeRuntimeData()
        {
            currentHealth = maxHealth;
            // Any other runtime initializations based on SO data
        }
    }
}