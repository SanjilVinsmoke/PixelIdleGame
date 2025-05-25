using System;
using System.Collections.Generic;
using System.Linq;
using Component;
using Component.Interfaces;
using ScriptableObjects;
using StateMachine;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Managers
{
    public enum EnemyEvent
    {
        Idle,
        Move,
        Attack,
        Hit,
        Death,
        Roam,
        Patrol,
        Flee,
    }
    
    public abstract class Enemy<T> :MonoBehaviour, IDamageable where T : Enemy<T>
    {
        [SerializeField]
        private bool debugMode = false;
        [Header("Enemy Settings")]
        public EnemyDataSo enemyData;
        
        public Vector3 lastAttackPosition;
        // Enable debug mode for state machine
        // Auto-injected components
        [AutoRequire] public MoveComponent      movementComponent;
        [AutoRequire] public AttackComponent    attackComponent;
        [AutoRequire] public HealthComponent    healthComponent;
        [AutoRequire] public AnimationComponent animationComponent;
        
        protected StateMachine<T, EnemyEvent> stateMachine;
        private bool playerDetected = false;
        
        // Current State
        public Type CurrentState => stateMachine.CurrentState?.GetType();

        protected virtual void Awake()
        {
           
            ComponentInjector.InjectComponents(this);
            animationComponent.animator = transform.GetChild(0).GetComponent<Animator>();
            StateMachine<T, EnemyEvent>.DebugMode = debugMode;
           
            healthComponent.OnDeath += Die;
            ApplyEnemyData();
        }

        protected virtual void Start()
        {
            // Configure state machine event-to-state mappings
            stateMachine = new StateMachine<T, EnemyEvent>((T)this, sm =>
            {
                ConfigureEventMappings(sm);
            });
            
            AddStates();
        }
        
        private void ApplyEnemyData()
        {
            if (enemyData == null) return;
            
            // Apply health settings
            if (healthComponent != null)
            {
                healthComponent.Health = enemyData.maxHealth;
               
            }
            
            // Apply movement settings
            if (movementComponent != null)
            {
                movementComponent.speed = enemyData.moveSpeed;
               
            }
            
            // Apply attack settings
            if (attackComponent != null)
            {
               //TODO: Implement attack component settings
               // attackComponent.attackDamage = enemyData.attackDamage;
                attackComponent.attackRange = enemyData.attackRange;
                attackComponent.attackCooldown = enemyData.attackCooldown;
            }
            
            // Apply animation settings
            if (animationComponent != null && enemyData.animatorController != null)
            {
                var animator = GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = enemyData.animatorController;
                    animator.speed = enemyData.animationSpeed;
                }
            }
        }
        
        private void ConfigureEventMappings(StateMachine<T, EnemyEvent> sm)
        {
            if (enemyData?.availableStates == null) return;
            
            foreach (var stateData in enemyData.availableStates)
            {
                if (!stateData.isEnabled) continue;
                
                var stateEvent = stateData.stateEvent;
                var stateClassName = stateData.stateClassName;
                
                // Search for the type in all loaded assemblies
                System.Type stateType = FindStateType(stateClassName);
                
                if (stateType != null)
                {
                    // Create generic type with T
                    var genericStateType = stateType.MakeGenericType(typeof(T));
                    
                    sm.AddEventMapping(stateEvent, () => {
                        var method = typeof(StateMachine<T, EnemyEvent>).GetMethod("ChangeState");
                        var genericMethod = method.MakeGenericMethod(genericStateType);
                        genericMethod.Invoke(sm, null);
                    });
                }
                else
                {
                    Debug.LogWarning($"State class '{stateClassName}' not found for event mapping. Make sure the class exists in the StateMachine namespace.");
                }
            }
        }
        
        protected virtual void AddStates()
        {
            if (enemyData?.availableStates == null) return;
            
            foreach (var stateData in enemyData.availableStates)
            {
                if (!stateData.isEnabled) continue;
                
                var stateClassName = stateData.stateClassName;
                
                // Search for the type in all loaded assemblies
                System.Type stateType = FindStateType(stateClassName);
                
                if (stateType != null)
                {
                    try
                    {
                        // Create generic type with T
                        var genericStateType = stateType.MakeGenericType(typeof(T));
                        var stateInstance = System.Activator.CreateInstance(genericStateType);
                        
                        // Add state to state machine using reflection
                        var addStateMethod = typeof(StateMachine<T, EnemyEvent>).GetMethod("AddState");
                        addStateMethod.Invoke(stateMachine, new[] { stateInstance });
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Failed to create state {stateClassName}: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"State class '{stateClassName}' not found. Make sure the class exists in the StateMachine namespace.");
                }
            }
            
            // Set default state from ScriptableObject
            if (enemyData != null)
            {
                stateMachine.ProcessEvent(enemyData.defaultState);
            }
        }

        protected virtual void Update()
        {
            stateMachine.Update();
            Debug.Log($"Current State: {stateMachine.CurrentState?.GetType().Name}");
            CheckForPlayer();
        }

        protected virtual void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void TakeDamage(float damage, Vector3  hitPosition)
        {
            if (healthComponent.IsDead)
                return;
            lastAttackPosition = hitPosition;
            healthComponent.TakeDamage(damage);
            stateMachine.ProcessEvent(EnemyEvent.Hit);
        }
      
        public  virtual void   TakeDamage(float damage)
        {
            TakeDamage(damage, transform.position); // fallback to own position
        }


        public void TakeDamage(float damage, Vector2 hitDirection)
        {
            
        }

        public virtual void Die()
        {
            stateMachine.ProcessEvent(EnemyEvent.Death);
        }

        public virtual void OnHit()
        {
            stateMachine.ProcessEvent(EnemyEvent.Hit);
        }
        
        protected virtual void CheckForPlayer()
        {
            if (enemyData == null) return;
            
            Collider[] hits = Physics.OverlapSphere(transform.position, enemyData.detectionRange, enemyData.playerLayer);
            bool previouslyDetected = playerDetected;
            playerDetected = hits.Length > 0;
            
            if (playerDetected && !previouslyDetected)
            {
                stateMachine.ProcessEvent(EnemyEvent.Move);
            }
            else if (!playerDetected && previouslyDetected)
            {
                stateMachine.ProcessEvent(EnemyEvent.Idle);
            }
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            if (enemyData == null) return;
            
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
            // Draw attack range
            Gizmos.color = Color.red;
            if (attackComponent != null && attackComponent.attackPoint != null)
            {
                Gizmos.DrawWireSphere(attackComponent.attackPoint.position, attackComponent.attackRange);
            }
        }
        
        // Helper method to find state types, checking both generic and non-generic variants
        private System.Type FindStateType(string stateClassName)
        {
            System.Type stateType = null;
            
            // Try direct approach first
            stateType = System.Type.GetType($"StateMachine.{stateClassName}");
            
            if (stateType == null)
            {
                // Try with generic notation
                stateType = System.Type.GetType($"StateMachine.{stateClassName}`1");
            }
            
            // If still not found, search through all assemblies
            if (stateType == null)
            {
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    // Try exact match
                    stateType = assembly.GetType($"StateMachine.{stateClassName}");
                    if (stateType != null) break;
                    
                    // Try generic match
                    stateType = assembly.GetType($"StateMachine.{stateClassName}`1");
                    if (stateType != null) break;
                    
                    // Try searching all types
                    var types = assembly.GetTypes()
                        .Where(t => t.Namespace == "StateMachine" && 
                                   (t.Name == stateClassName || t.Name == $"{stateClassName}`1"));
                    
                    if (types.Any())
                    {
                        stateType = types.First();
                        break;
                    }
                }
            }
            
            return stateType;
        }
    }
}