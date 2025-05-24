
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    public float speed =0 ;
    
    public Rigidbody2D rb;

    private bool isFacingRight = true;

    private void Awake()
    {
       rb = GetComponent<Rigidbody2D>();
       
    }

    /// <summary>
    /// Applies horizontal movement based on input (-1 to 1).
    /// If input is zero, stops velocity immediately to prevent sliding.
    /// </summary>
    public void Move(float horizontalInput)
    {
        // Instant stop when no input
        if (Mathf.Approximately(horizontalInput, 0f))
        {
            Vector2 vel = rb.linearVelocity;
            vel.x = 0f;
            rb.linearVelocity = vel;
            return;
        }

        // Regular movement
        Vector2 velocity = rb.linearVelocity;
        velocity.x = horizontalInput * speed;
        rb.linearVelocity = velocity;

        HandleFlip(horizontalInput);
    }

    /// <summary>
    /// Applies a friction factor (0 to 1) to horizontal velocity.
    /// Optionally, use PhysicsMaterial2D or Rigidbody2D.drag instead.
    /// </summary>
    public void ApplyFriction(float friction)
    {
        Vector2 vel = rb.linearVelocity;
        vel.x *= (1f - friction);
        rb.linearVelocity = vel;
    }

    private void HandleFlip(float horizontalInput)
    {
        if (horizontalInput > 0 && !isFacingRight)
            Flip();
        else if (horizontalInput < 0 && isFacingRight)
            Flip();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
}