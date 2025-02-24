using System;
using UnityEngine;

namespace Component.Interfaces
{
    public class HealthComponent : MonoBehaviour, IHealthComponent
    {
        public Action<float> OnHealthChanged { get; set; }
        public Action OnDeath { get; set; }
        public float Health { get; set; }
        
        // Add Scritable Object 
        public void TakeDamage(float damage)
        {
            Health -= damage;
           // sendEvent
            if (Health <= 0)
            {
                //SendEvent
            }
        }

        public void Heal(float healAmount)
        {
            throw new NotImplementedException();
        }
    }
}