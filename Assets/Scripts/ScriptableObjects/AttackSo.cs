using UnityEngine;

namespace ScriptableObjects
{
    public class AttackSo
    {
        [Header("Attack Properties")]
        public int damageAmount = 10;
        public Transform attackPoint;
        public float attackRange = 1f;
        public LayerMask damageableLayer;
    }
    
}