using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Enemy Template", menuName = "Enemy System/Enemy Template")]
    public class EnemyTemplate : ScriptableObject
    {
        [Header("Template Info")]
        public string templateName;
        public string description;
        public Sprite icon;
        public EnemyType enemyType;
        
        [Header("Default Settings")]
        public EnemyDataSo defaultData;
        public GameObject basePrefab;
        
        [Header("Required Components")]
        public List<string> requiredComponents = new List<string>
        {
            "MoveComponent",
            "AttackComponent", 
            "HealthComponent",
            "AnimationComponent"
        };
        
        [Header("Default States")]
        [Tooltip("Configure which states this enemy template should have")]
        public List<EnemyStateData> defaultStates = new List<EnemyStateData>
        {
            new EnemyStateData { stateEvent = EnemyEvent.Idle, stateClassName = "EnemyIdleState", isEnabled = true },
            new EnemyStateData { stateEvent = EnemyEvent.Move, stateClassName = "EnemyMoveState", isEnabled = true },
            new EnemyStateData { stateEvent = EnemyEvent.Attack, stateClassName = "EnemyAttackState", isEnabled = true },
            new EnemyStateData { stateEvent = EnemyEvent.Hit, stateClassName = "EnemyHitState", isEnabled = true },
            new EnemyStateData { stateEvent = EnemyEvent.Death, stateClassName = "EnemyDeathState", isEnabled = true },
        };

                private void ApplyTypeSpecificDefaults()
        {
            if (defaultData == null) return;

            switch (enemyType)
            {
                case EnemyType.Worm:
                    defaultData.touchDamage = 15f;
                    defaultData.hasTouchDamage = true;
                    defaultData.moveSpeed = 1f;
                    defaultData.maxHealth = 30f;
                    break;
                case EnemyType.Bat:
                    defaultData.moveSpeed = 5f;
                    defaultData.maxHealth = 20f;
                    defaultData.detectionRange = 10f;
                    break;
                case EnemyType.Spider:
                    defaultData.moveSpeed = 3f;
                    defaultData.maxHealth = 40f;
                    defaultData.attackDamage = 20f;
                    break;
                // Add more type-specific defaults as needed
            }
        }
    }
}
