using UnityEngine;
using System;

public class JumpComponent : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float inputBufferTime = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private int jumpCount;
    private float lastJumpRequestTime = Mathf.NegativeInfinity;

    public event Action OnLanded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError($"[JumpComponent] No Rigidbody found on {gameObject.name}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only count collisions with objects below us as landing
        if (Vector2.Dot(collision.GetContact(0).normal, Vector2.up) > 0.5f)
        {
            isGrounded = true;
            jumpCount = 0;
            
            // Notify state machine we've landed
            OnLanded?.Invoke();
            
            // replay buffered jump if pressed just before landing
            if (Time.time - lastJumpRequestTime <= inputBufferTime)
                PerformJump();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    // called by state in response to input
    public void HandleJumpInput()
    {
        lastJumpRequestTime = Time.time;
        PerformJump();
    }

    // internal jump logic
    private void PerformJump()
    {
        if (rb != null && jumpCount < maxJumps)
        {
            // Use velocity directly instead of AddForce for more consistent behavior
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }
    }
}
