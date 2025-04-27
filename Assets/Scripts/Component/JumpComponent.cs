using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpComponent : MonoBehaviour
{
    [Header("Jump Heights")]
    [Tooltip("Peak height of a full jump (in Unity units)")]
    [SerializeField] private float maxJumpHeight = 3f;
    [Tooltip("Minimum height if player releases early")]
    [SerializeField] private float minJumpHeight = 1f;

    [Header("Extra Jumps")]
    [Tooltip("How many jumps beyond the first (e.g. 1 = double-jump)")]
    [SerializeField] private int maxExtraJumps = 1;

    [Header("Coyote Time")]
    [Tooltip("Seconds after leaving ground that you can still jump")]
    [SerializeField] private float coyoteTimeThreshold = 0.1f;

    [Header("Gravity Multipliers")]
    [Tooltip("Gravity scale when falling")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [Tooltip("Gravity scale when rising but jump button released")]
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    // runtime
    private Rigidbody2D rb;
    private bool          isGrounded;
    private bool          wasGroundedLastFrame;
    private int           extraJumps;
    private float         timeSinceGrounded = Mathf.Infinity;
    private bool          jumpReleased;

    public event Action OnLanded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        extraJumps = maxExtraJumps;
        if (groundCheckPoint == null)
            Debug.LogWarning($"[{nameof(JumpComponent)}] GroundCheckPoint not assigned on {gameObject.name}.");
    }

    private void FixedUpdate()
    {
        UpdateGroundState();
        ApplyGravityModifiers();
    }

    private void UpdateGroundState()
    {
        wasGroundedLastFrame = isGrounded;

        isGrounded = groundCheckPoint != null &&
            Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0f, groundLayer);

        if (isGrounded)
        {
            // reset counters on ground
            timeSinceGrounded = 0f;
            extraJumps = maxExtraJumps;
            if (!wasGroundedLastFrame)
                OnLanded?.Invoke();
        }
        else
        {
            timeSinceGrounded += Time.fixedDeltaTime;
        }
    }

    private void ApplyGravityModifiers()
    {
        if (rb.linearVelocity.y < 0f)
        {
            // faster falling
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0f && jumpReleased)
        {
            // short hop if released early
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    /// <summary>
    /// Call this to attempt a jump (e.g. on button-down).
    /// Honors ground, coyote time, and extra-jumps.
    /// </summary>
    public void Jump()
    {
        bool canUseCoyote = timeSinceGrounded <= coyoteTimeThreshold;
        if (isGrounded || canUseCoyote || extraJumps > 0)
        {
            // consume extra jump only if truly airborne (not coyote)
            if (!isGrounded && !canUseCoyote)
                extraJumps--;

            // Always use default gravity scale for jump calculation
            float g = Physics2D.gravity.y * 1f;
            float v0 = Mathf.Sqrt(-2f * g * maxJumpHeight);

            // apply jump
            Vector2 vel = rb.linearVelocity;
            vel.y = v0;
            rb.linearVelocity = vel;

            jumpReleased = false;
            // force exit coyote window
            timeSinceGrounded = coyoteTimeThreshold + 1f;
        }
    }

    /// <summary>
    /// Call this to shorten the jump early (e.g. on button-up).
    /// </summary>
    public void EndJump()
    {
        jumpReleased = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}
