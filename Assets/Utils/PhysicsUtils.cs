using UnityEngine;


namespace Utils
{
    public class PhysicsUtils
    {
      public static bool IsGrounded(Rigidbody rb, float groundCheckDistance = 0.1f)
      {
          return Physics.Raycast(rb.position, Vector3.down, groundCheckDistance);
      }
      public static bool IsGrounded(Rigidbody2D rb, float groundCheckDistance = 0.1f)
      {
          return Physics2D.Raycast(rb.position, Vector2.down, groundCheckDistance);
      }
      
      public static void DrawRay(Vector3 origin, Vector3 direction, Color color, float duration = 0f)
      {
          Debug.DrawRay(origin, direction, color, duration);
      }
      public static void DrawRay(Ray ray, Color color, float duration = 0f)
      {
          Debug.DrawRay(ray.origin, ray.direction, color, duration);
      }
      
      public static bool IsFacingWall(Rigidbody rb, float distance = 0.1f)
      {
          RaycastHit hit;
          if (Physics.Raycast(rb.position, rb.transform.forward, out hit, distance))
          {
              return true;
          }
          return false;
      }
      public static bool IsFacingWall(Rigidbody2D rb, float distance = 0.1f)
      {
          RaycastHit2D hit = Physics2D.Raycast(rb.position, rb.transform.right, distance);
          return hit.collider != null;
      }
    }
}