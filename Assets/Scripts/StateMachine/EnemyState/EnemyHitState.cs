
using Constant;
using Managers;
using UnityEngine;
using Utils;

namespace StateMachine
{
    [StateDescription("Enemy is idle, watching the player")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Yellow)]
    public class EnemyHitState<T>:BaseState<T, EnemyEvent> where T : Enemy<T>
    {
        private float knockBackTimer;
        private Vector2 knockBackDirection;
        
        public override void Enter()
        {
            base.Enter();
            knockBackTimer = owner.enemyData.knockbackDuration;
            knockBackDirection = (owner.transform.position - owner.lastAttackPosition).normalized;

        }

        public override void Update()
        {
            base.Update();
            
            

        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}