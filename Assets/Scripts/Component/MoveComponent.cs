using ScriptableObjects;
using Unity.VisualScripting;
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
    
    
    
    
    
    
    
}
