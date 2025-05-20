using System;
using System.Collections.Generic;
using Component;
using Component.Interfaces;
using ScriptableObjects;
using StateMachine;
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
    }
    
    public abstract class Enemy<T> :MonoBehaviour, IDamageable where T : Enemy<T>
    {
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
            StateMachine<T, EnemyEvent>.DebugMode = debugMode;
            
            healthComponent.OnDeath += Die;
        }

        protected virtual void Start()
        {
            // Configure state machine event-to-state mappings
            stateMachine = new StateMachine<T, EnemyEvent>((T)this, sm =>
            {  sm.AddEventMapping(EnemyEvent.Idle, () => sm.ChangeState<EnemyIdleState<T>>());
             
            });
            
            
        }
        
        protected virtual void AddStates()
        {
           stateMachine.AddState ( new EnemyIdleState<T>());
            
           
        }
        
        
        protected  virtual void Update()
        {
            stateMachine.Update();
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
           
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            
            
        }
    }
}