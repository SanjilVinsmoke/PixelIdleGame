using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Entity/Health", menuName = "Entity", order = 0)]
    public class HealthSo : ScriptableObject
    {
        
        public float health = 100;
        public float maxHealth = 100;
        public bool isDead = false;
    }
}