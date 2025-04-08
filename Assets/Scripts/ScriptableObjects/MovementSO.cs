using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Movement", menuName = "ScriptableObjects/MovementSO", order = 0)]
    public class MovementSo : ScriptableObject
    {
        public float speed = 5f;
        public float jumpForce = 10f;
        public float gravityScale = 1f;
        public float groundCheckDistance = 0.1f;
        public LayerMask groundLayer;

        // Add any other movement-related properties here
        
    }
}