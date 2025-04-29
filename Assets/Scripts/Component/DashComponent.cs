using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashComponent : MonoBehaviour
{
    [Header("Dash Settings")]
    [Tooltip("How fast the dash moves (units/sec)")]
    [SerializeField] private float dashSpeed = 15f;
    [Tooltip("How far the dash travels (units)")]
    [SerializeField] private float dashDistance = 5f;
    [Tooltip("Cooldown time between dashes (sec)")]
    [SerializeField] private float dashCooldown = 1f;
    [Tooltip("How many air-dashes you get")]
    [SerializeField] private int maxAirDashes = 1;
    [Tooltip("Disable gravity during dash?")]
    [SerializeField] private bool disableGravityDuringDash = true;

    private Rigidbody2D    rb;
    private JumpComponent  jumpComp;
    private float          lastDashTime = -Mathf.Infinity;
    private int            airDashesRemaining;
    private Coroutine dashCoroutine;

    public bool IsDashing { get; private set; }
    public event Action OnDashFinished;

    private void Awake()
    {
        rb                 = GetComponent<Rigidbody2D>();
        jumpComp           = GetComponent<JumpComponent>();
        airDashesRemaining = maxAirDashes;

        // reset air-dashes when you land
        if (jumpComp != null)
        {
            jumpComp.OnLanded += () => airDashesRemaining = maxAirDashes;
            jumpComp.OnLanded += HandleLandedDuringDash; // Add this line
        }
    }

    /// <summary>
    /// Returns true if cooldown is ready and you have any dash charges (ground or air).
    /// </summary>
    public bool CanDash()
    {
        bool offCooldown = Time.time >= lastDashTime + dashCooldown;
        bool inAir       = jumpComp != null && !jumpComp.isGrounded;
        bool hasAirDash  = airDashesRemaining > 0;
        return offCooldown && inAir && hasAirDash;
    }

    /// <summary>
    /// Start a dash in the given world‐space direction.  
    /// If direction is zero, will dash in your current facing (x‐scale).
    /// </summary>
    private bool wasGroundedAtDashStart = false;
    private bool dashInterrupted = false;

    public void Dash(Vector2 direction)
    {
        if (!CanDash()) return;
        lastDashTime = Time.time;
    
        // consume an air-dash if not grounded
        if (jumpComp != null && !jumpComp.isGrounded)
            airDashesRemaining--;

        wasGroundedAtDashStart = jumpComp != null && jumpComp.isGrounded;
        dashInterrupted = false;

        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(DashRoutine(direction));
    }

    private void HandleLandedDuringDash()
    {
        // Only interrupt dash early if it was an air dash
        if (IsDashing && !wasGroundedAtDashStart)
        {
            dashInterrupted = true;
        }
    }

    private IEnumerator DashRoutine(Vector2 dir)
    {
        IsDashing = true;
        float origGravity = rb.gravityScale;
        if (disableGravityDuringDash) rb.gravityScale = 0f;

        // choose default if no input
        if (dir.sqrMagnitude < 0.01f)
            dir = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
        dir.Normalize();

        // dash for exactly distance/speed seconds
        float duration = dashDistance / dashSpeed;
        float endTime  = Time.time + duration;
        while (Time.time < endTime && !dashInterrupted)
        {
            rb.linearVelocity = dir * dashSpeed;
            yield return null;
            // Only break if this was an air dash and we landed
            if (!wasGroundedAtDashStart && jumpComp != null && jumpComp.isGrounded)
                break;
        }

        // restore
        if (disableGravityDuringDash) rb.gravityScale = origGravity;
        rb.linearVelocity = Vector2.zero; // Stop sliding
        IsDashing = false;
        OnDashFinished?.Invoke();
    }
}
