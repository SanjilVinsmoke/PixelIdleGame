using System;
using ScriptableObjects;
using UnityEngine;

namespace Component.Interfaces
{
    public class HealthComponent : MonoBehaviour, IHealthComponent
    {
        public Action<float> OnHealthChanged { get; set; }
        public Action OnDeath { get; set; }

        public float Health { get; set; }
        [SerializeField]
        private HealthSo healthValues;

        public bool IsDead = false;
        private void Awake()
        {
           
            Health = healthValues.health;
            
            
        }
        public void TakeDamage(float damage)
        {
            
            Health -= damage;
            
           // sendEvent
            if (Health <= 0)
            {
                IsDead = true;
                OnDeath?.Invoke();
            }
        }

        public void Heal(float healAmount)
        {
            Health += healAmount;
            OnHealthChanged?.Invoke(Health);


        }
    }
}