using System;
using Component.Interfaces;
using UnityEngine;

namespace Damage
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageDealer:MonoBehaviour
    {
        
        [Tooltip("Which layers can be damaged")]
        public LayerMask targetMask;
        
        private float damageAmount =10;

        private void OnCollisionEnter2D(Collision2D other)
        {
          
            GameObject target = other.gameObject;

            if (((1 << target.layer) & targetMask) != 0)
            { Utils.DebugUtils.DrawCircle( other.transform.position,2 , Color.red);
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