using UnityEngine;

namespace Component.Interfaces
{
    public class HealthComponent : MonoBehaviour, IHealthComponent
    {
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
    }
}