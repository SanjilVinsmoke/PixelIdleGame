// EnemyDataSo.cs
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemies/Enemy Data", order = 1)]
    public class EnemyDataSo : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Human-readable name for this enemy type")]
        public string enemyName = "New Enemy";

        [Header("Availability")]
        [Tooltip("If false, this enemy will be ignored / not spawned at runtime")]
        public bool isAvailable = true;

        [Header("Visuals")]
        [Tooltip("Sprite used to represent this enemy")]
        public Sprite enemySprite;

        [Header("Detection Settings")]
        [Tooltip("How far the enemy can 'see' the player")]
        public float playerDetectionRadius = 5f;
        public LayerMask playerLayer;

        [Header("Waypoint Settings")]
        [Tooltip("Optional patrol waypoints. Leave empty to disable patrolling.")]
        public Transform[] waypoints;

        [Header("Movement & Combat")]
        [Tooltip("How fast this enemy moves")]
        public float moveSpeed = 3f;
        public LayerMask groundLayer;
        public LayerMask wallLayer;
        
        [Tooltip("Total health points")]
        public int maxHealth = 10;

        [Header("Attack Settings")]
        [Tooltip("Damage per hit")]
        public int damage = 1;
        [Tooltip("Cooldown between attacks (seconds)")]
        public float attackCooldown = 1f;
        
        
        [Header("Knockback Settings")]
        [Tooltip("Knockback force applied to the enemy when hit")]
        public float knockbackForce = 5f;
        [Tooltip("Knockback duration (seconds)")]
        public float knockbackDuration = 0.5f;

      
        
    }
}