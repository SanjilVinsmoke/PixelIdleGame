using Component.Interfaces;
using UnityEngine;



namespace Component
{
    public class AttackComponent : MonoBehaviour, Interfaces.IBaseAttackComponent
    {

        private float lastAttackTime = -Mathf.Infinity;

        // Check if the cooldown has passed
        public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;

        public float attackCooldown = 1f;

        
        [Header("Attack Properties")]
        public int damageAmount = 100;
        public Transform attackPoint;
        public float attackRange = 1f;
        public LayerMask damageableLayer;
        


        public void Attack()
        {
            if (CanAttack)
            {
                Debug.Log("Attacking!");
                lastAttackTime = Time.time;
                DetectDamageable();
            
            }
        }

        private void DetectDamageable()
        {
            if(attackPoint == null) {
                attackPoint = transform;
                Debug.LogWarning("Attack point not set. Defaulting to transform.");
        
            }
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, damageableLayer);

            foreach (Collider2D hitObject in hitObjects)
            {
                Debug.Log("Hit: " + hitObject.name); 
                IDamageable damagable = hitObject.GetComponent<IDamageable>();
                if (damagable != null)
                {
                    damagable.TakeDamage(damageAmount);
                }
            }

     
        }
        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}