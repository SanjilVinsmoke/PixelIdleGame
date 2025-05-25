// EnemyDataSo.cs
using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utils;

namespace ScriptableObjects
{
    public enum EnemyType
    {
        Worm,
        Bat,
        Spider,
        Goblin,
        Orc,
        Skeleton,
        Zombie,
        Ghost,
        Golem,
        Dragon
    }

    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy System/Enemy Data")]
    public class EnemyDataSo : ScriptableObject
    {
        [Header("Enemy Type")]
        public EnemyType enemyType;
        
        [Header("Basic Info")]
        public string enemyName;
        public GameObject enemyPrefab;
        
        [Header("Core Stats")]
        public float maxHealth = 100f;
        public bool regenerateHealth = false;
        public float healthRegenRate = 5f;
        
        [Header("Animation Settings")]
        public RuntimeAnimatorController animatorController;
        public float animationSpeed = 1f;
        
        [Header("Movement Settings")]
        public float moveSpeed = 3f;
        public float rotationSpeed = 180f;
        
        [Header("Attack Settings")]
        public float attackDamage = 25f;
        public float attackRange = 2f;
        public float attackCooldown = 1.5f;
        
        [Header("Touch Damage")]
        [Tooltip("Damage dealt immediately on touch (e.g. Worm)")]
        public float touchDamage = 10f;
        public bool hasTouchDamage = false;
        
        [Header("Detection Settings")]
        public float detectionRange = 8f;
        public LayerMask playerLayer = 1;
        
        [Header("State Machine Settings")]
        public List<EnemyStateData> availableStates = new List<EnemyStateData>();
        public EnemyEvent defaultState = EnemyEvent.Idle;

        [Header("Layer Settings")] 
        public LayerMask groundLayer;
        public LayerMask wallLayer;

        
        [Header("Knockback Settings")]
        public float knockbackForce = 5f;
        public float knockbackDuration = 0.5f;
        
        [Header("Collider Settings")]
        public Enums.Collider2DType colliderType = Enums.Collider2DType.BoxCollider2D;
     
        // Legacy property for backward compatibility
        public float detectionRadius 
        { 
            get => detectionRange; 
            set => detectionRange = value; 
        }
        
      
    }
    
    [Serializable]
    public class EnemyStateData
    {
        public EnemyEvent stateEvent;
        public string stateClassName;
        public bool isEnabled = true;
        public float statePriority = 1f;
    }
}