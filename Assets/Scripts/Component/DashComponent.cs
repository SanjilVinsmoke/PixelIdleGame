using UnityEngine;
using System.Collections;

public class DashComponent : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private bool ignoreGravityDuringDash = true;

    private Rigidbody2D rb;
    private MoveComponent moveComponent; // Assuming MoveComponent handles facing direction
    private float lastDashTime = Mathf.NegativeInfinity;
    private bool isDashing = false;
    private float originalGravityScale;

    public bool IsDashing => isDashing;
    public bool CanDash => Time.time >= lastDashTime + dashCooldown && !isDashing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveComponent = GetComponent<MoveComponent>();
        if (rb == null)
            Debug.LogError($"[DashComponent] No Rigidbody found on {gameObject.name}");
        if (moveComponent == null)
            Debug.LogWarning($"[DashComponent] No MoveComponent found on {gameObject.name}. Directional dash might not work as expected.");
        originalGravityScale = rb.gravityScale;
    }

    public void PerformDash()
    {
        if (!CanDash || rb == null)
            return;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float dashDirection = transform.localScale.x > 0 ? 1f : -1f; // Use localScale for direction
        Vector2 dashVelocity = new Vector2(dashDirection * dashForce, 0f);

        if (ignoreGravityDuringDash)
        {
            rb.gravityScale = 0f;
        }
        rb.linearVelocity = dashVelocity; // Use velocity for instant dash

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero; // Stop dash abruptly or apply friction?
        if (ignoreGravityDuringDash)
        {
            rb.gravityScale = originalGravityScale;
        }
        isDashing = false;
    }

    // Optional: Method to cancel dash early if needed
    public void CancelDash()
    {
        if (isDashing)
        {
            StopCoroutine(nameof(DashCoroutine));
            rb.linearVelocity = Vector2.zero;
            if (ignoreGravityDuringDash)
            {
                 rb.gravityScale = originalGravityScale;
            }
            isDashing = false;
        }
    }
}