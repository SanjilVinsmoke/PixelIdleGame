using UnityEngine;

namespace Damage
{
    public class DamageDealer:MonoBehaviour
    {
        
        [Tooltip("Which layers can be damaged")]
        public LayerMask targetMask;
        
        private float damageAmount;
        
        
    }
}