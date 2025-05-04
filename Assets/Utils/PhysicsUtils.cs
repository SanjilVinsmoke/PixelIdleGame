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
      
      
      public static bool IsGrounded(Rigidbody2D rb , LayerMask groundLayer,float groundCheckDistance = 0.1f , bool debugMode = false)
      {
          if (debugMode)
          {
              Debug.DrawRay(rb.position, Vector2.down * groundCheckDistance, Color.yellow);
          }
          
          // Check if the raycast hits the ground layer
          return Physics2D.Raycast(rb.position, Vector2.down, groundCheckDistance, groundLayer);
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

      public static bool IsFacingWall(Rigidbody2D rb, float distance = 0.1f, bool debugMode = false)
      {
          if (debugMode)
          {
              Debug.DrawRay(rb.position, rb.transform.right * distance, Color.red);
          }

          {
              if (Physics2D.Raycast(rb.position, rb.transform.right, distance))
              {
                  return true;
              }

              return false;
          }
      }

      public static bool IsFacingWall(Rigidbody2D rb, LayerMask wallLayer, float distance = 0.1f , bool debugMode = false)
         {
             if (debugMode)
             {
                 Debug.DrawRay(rb.position, rb.transform.right * distance, Color.red);
             }
             if (Physics2D.Raycast(rb.position, rb.transform.right, distance, wallLayer))
             {
                 return true;
             }
             return false;
         }
    }
}