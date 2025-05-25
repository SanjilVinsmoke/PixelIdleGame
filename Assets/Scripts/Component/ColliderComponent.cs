using UnityEngine;

namespace Component
{
    [RequireComponent(typeof(Collider2D))]
    public class ColliderComponent : MonoBehaviour
    {
        private Collider2D col;
        private SpriteRenderer spriteRenderer;
        
        [SerializeField] private float colliderSizeMultiplier = 0.9f;
        [SerializeField] private Vector2 colliderOffset = Vector2.zero;
        
        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            UpdateColliderToSprite();
        }
        
        public void UpdateColliderToSprite()
        {
            if (col == null || spriteRenderer == null) return;
            
            if (col is BoxCollider2D boxCollider)
            {
                boxCollider.size = spriteRenderer.bounds.size * colliderSizeMultiplier;
                boxCollider.offset = colliderOffset;
            }
            else if (col is CapsuleCollider2D capsuleCollider)
            {
                capsuleCollider.size = spriteRenderer.bounds.size * colliderSizeMultiplier;
                capsuleCollider.offset = colliderOffset;
            }
        }
        
        public void SetCustomColliderSize(Vector2 size, Vector2 offset)
        {
            if (col == null) return;


            if (col is BoxCollider2D boxCollider)
            {
                boxCollider.size = size;
                boxCollider.offset = offset;
            }
            else if (col is CapsuleCollider2D capsuleCollider)
            {
                capsuleCollider.size = size;
                capsuleCollider.offset = offset;
            }
        }
    }
}