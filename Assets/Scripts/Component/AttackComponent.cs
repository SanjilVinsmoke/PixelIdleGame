using UnityEngine;

namespace Component
{
    public class AttackComponent : MonoBehaviour, Interfaces.IBaseAttackComponent
    {
        public float attackCooldown = 1f;
        private float lastAttackTime = -Mathf.Infinity;

        // Check if the cooldown has passed
        public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;

        public void Attack()
        {
            if (CanAttack)
            {
                Debug.Log("Attacking!");
                lastAttackTime = Time.time;
               
            }
        }
    }
}