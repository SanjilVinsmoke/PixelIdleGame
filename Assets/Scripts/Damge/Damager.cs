using System;
using Component.Interfaces;
using UnityEngine;

namespace Damage
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageDealerComponent:MonoBehaviour
    {
        
        [HideInInspector]
        public LayerMask targetMask;
        [HideInInspector]
        public float damageAmount =10;
        [HideInInspector]
        public float damageArea = 1f;
        private void OnCollisionEnter2D(Collision2D other)
        {
          
            GameObject target = other.gameObject;

            if (((1 << target.layer) & targetMask) != 0)
            { Utils.DebugUtils.DrawCircle( other.transform.position,damageArea , Color.red);
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