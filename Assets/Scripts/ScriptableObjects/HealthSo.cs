using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewHealth", menuName = "ScriptableObjects/Health", order = 0)]
    public class HealthSo : ScriptableObject
    {
        public float health = 100f;
        public float maxHealth = 100f;
        public bool isDead = false;
    }
}