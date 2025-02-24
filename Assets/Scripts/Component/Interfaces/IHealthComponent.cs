using System;
using System.Runtime.CompilerServices;

namespace Component.Interfaces
{
    public interface IHealthComponent
    {
        Action<float> OnHealthChanged { get; set; }
        Action OnDeath { get; set; }
        public float Health { get; set; }
        public void TakeDamage(float damage);
        
        public void Heal(float healAmount);
    }
}