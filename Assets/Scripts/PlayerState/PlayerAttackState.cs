

    using System;
    using UnityEngine;
    using Utils;

    [StateDescription("Player is attacking")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Red)]
    public class PlayerAttackState: BaseState<Player, PlayerEvent>
    { 
        
        private float attackStartTime;
        private bool attackPerformed { get; set; }

        public override void Enter()
        {
            base.Enter();
            attackPerformed = false;
            attackStartTime = Time.time;
            
            Attack();
          
        }

     
        private void Attack()
        {
            if (owner.attackComponent != null)
            {
                owner.attackComponent.Attack();
                attackPerformed = true;
            }
            
        }



        public override void Update()
        {
            base.Update();
     
            if(attackPerformed && Time.time >= attackStartTime + owner.attackComponent.attackCooldown)
            {
                stateMachine.ProcessEvent(PlayerEvent.Idle);
            }
           
           
        }
        
     
    
    }
   
