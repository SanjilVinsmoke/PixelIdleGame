
    using Constant;

    public class PlayerDeathState: BaseState<Player, PlayerEvent>
    {
        
        
        public override void Enter()
        {
            base.Enter();
            owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.DEATH);
            owner.movementComponent.Move(0f);
            owner.inputComponent.DisableInput();
            
            // Invoke death event for any listeners
            
        }
        
        
        public override void Exit()
        {
            base.Exit();
            owner.inputComponent.EnableInput();
        }
        
        
        
    }
