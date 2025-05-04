using System;
using System.Collections.Generic;
using Component;
using Component.Interfaces;
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
        
        [Header("Enemy Settings")]
        [SerializeField] public bool debugMode = false;
        [SerializeField] public float playerDetectionRadius = 5f;
        [SerializeField] public LayerMask playerLayer;
        
        [Header("Waypoint Settings")]
        [SerializeField] public Transform[] waypoints;
        [SerializeField] public float waypointStopDistance = 0.1f;
        [SerializeField] public bool loopWaypoints = true;
        
        // Enable debug mode for state machine
        // Auto-injected components
        [AutoRequire] public MoveComponent      movementComponent;
        [AutoRequire] public AttackComponent    attackComponent;
        [AutoRequire] public HealthComponent    healthComponent;
        [AutoRequire] public AnimationComponent animationComponent;
        
        protected StateMachine<T, EnemyEvent> stateMachine;
        private bool playerDetected = false;
        
        // Current State
        public System.Type CurrentState => stateMachine.CurrentState?.GetType();

        private void Awake()
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
        
        private void AddStates()
        {
           stateMachine.AddState ( new EnemyIdleState<T>());
            
           
        }
        
        
        private void Update()
        {
            stateMachine.Update();
            CheckForPlayer();
        }

        public void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void TakeDamage(float damage)
        {
            healthComponent.TakeDamage(damage);
            stateMachine.ProcessEvent(EnemyEvent.Hit);
        }

        public void Die()
        {
            stateMachine.ProcessEvent(EnemyEvent.Death);
        }

        public void OnHit()
        {
            stateMachine.ProcessEvent(EnemyEvent.Hit);
        }
        
        private void CheckForPlayer()
        {
            bool wasPlayerDetected = playerDetected;
            playerDetected = Physics2D.OverlapCircle(transform.position, playerDetectionRadius, playerLayer) != null;
            
            // If player was just detected, go to idle state
            if (playerDetected && !wasPlayerDetected)
            {
                stateMachine.ProcessEvent(EnemyEvent.Idle);
            }
            // If player is no longer detected, go back to roaming
            else if (!playerDetected && wasPlayerDetected)
            {
                stateMachine.ProcessEvent(EnemyEvent.Roam);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw the player detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
            
            // Draw waypoints and connections
            if (waypoints != null && waypoints.Length > 0)
            {
                Gizmos.color = Color.green;
                
                // Draw each waypoint
                foreach (var waypoint in waypoints)
                {
                    if (waypoint != null)
                        Gizmos.DrawSphere(waypoint.position, 0.2f);
                }
                
                // Draw lines between waypoints
                for (int i = 0; i < waypoints.Length - 1; i++)
                {
                    if (waypoints[i] != null && waypoints[i+1] != null)
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
                }
                
                // Draw line connecting last and first waypoint if looping
                if (loopWaypoints && waypoints.Length > 1 && waypoints[0] != null && waypoints[waypoints.Length-1] != null)
                {
                    Gizmos.DrawLine(waypoints[waypoints.Length-1].position, waypoints[0].position);
                }
            }
        }
    }
}