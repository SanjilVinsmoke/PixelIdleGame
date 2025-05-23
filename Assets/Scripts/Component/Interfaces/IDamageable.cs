using UnityEngine;

namespace Component.Interfaces
{
    public interface IDamageable
    {
        public void TakeDamage(float damage , Vector2 hitDirection);
        public void Die();
        
        public void OnHit();
        
    }
}