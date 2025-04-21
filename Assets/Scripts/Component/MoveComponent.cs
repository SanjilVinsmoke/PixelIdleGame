using ScriptableObjects;
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    [SerializeField]
    private MovementSo movementSo;
    
    [SerializeField]
    private Rigidbody2D rb;
    
    private  bool isGrounded;
    private bool isFacingRight = true;
    
    private void Awake()
    {
        if (movementSo == null)
        {
            Debug.LogError("MovementSo is not assigned in the inspector.");
        }
    }
    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    public void Move(float moveInput)
    {
        Vector2 movement = new Vector2(moveInput * movementSo.speed, rb.linearVelocity.y);
        rb.linearVelocity = movement;

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }
    public void ApplyFriction(float friction)
    {
        // Apply friction to the movement component
        Vector2 currentVelocity = rb.linearVelocity;
        float newVelocityX = currentVelocity.x * (1 - friction);
        rb.linearVelocity = new Vector2(newVelocityX, currentVelocity.y);
        
    }
    
    
    
    
    
    
    
}
