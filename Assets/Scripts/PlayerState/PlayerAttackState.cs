

    using Utils;

    [StateDescription("Player is attacking")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Red)]
    public class PlayerAttackState: BaseState<Player, PlayerEvent>
    {
        public override void Enter()
        {
            base.Enter();
            
            // Play the attack animation if an animator exists
            // if (owner.animatorComponent != null)
            // {
            //     owner.animatorComponent.Play("Attack");
            // }
        }

        // public override void Update()
        // {
        //     base.Update();
        //
        //     // Check for input to attack.
        //     if (Input.GetKeyDown(KeyCode.Space))
        //     {
        //         stateMachine.ProcessEvent(PlayerEvent.Attack); // Use event instead of direct state change
        //     }
        //
        //     // // Check if the player's health is depleted.
        //     // if (owner.healthComponent != null && owner.healthComponent.IsDead)
        //     // {
        //     //     stateMachine.ProcessEvent(PlayerEvent.Die);
        //     // }
        // }
    }
   
