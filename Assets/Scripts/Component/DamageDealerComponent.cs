using System;
using Component.Interfaces;
using UnityEngine;

namespace Component
{
    /// <summary>
    /// Component that deals damage to IDamageable targets on collision.
    /// Follows Single Responsibility Principle - only handles damage dealing logic.
    /// Uses LayerMask for efficient collision filtering.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DamageDealerComponent : MonoBehaviour
    {
        [Header("Damage Settings")]
        [Tooltip("Layer mask for targets that can be damaged")]
        [HideInInspector]
        public LayerMask targetMask;
        
        [Tooltip("Amount of damage to deal")]
        [HideInInspector]
        public float damageAmount = 10;
        
        [Tooltip("Area of effect for damage visualization")]
        [HideInInspector]
        public float damageArea = 1f;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            GameObject target = other.gameObject;

            // Check if target is on the specified layer
            if (((1 << target.layer) & targetMask) != 0)
            { 
                // Debug visualization
                Utils.DebugUtils.DrawCircle(other.transform.position, damageArea, Color.red);
                
                // Apply damage to IDamageable
                var damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 hitDirection = (target.transform.position - transform.position).normalized;
                    damageable.TakeDamage(damageAmount, hitDirection);
                }
            }
        }
    }
}

