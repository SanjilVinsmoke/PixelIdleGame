

    using Constant;
    using UnityEngine;
    using Utils;

    [StateDescription("Player is performing a down smash attack")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Magenta)]
    public class PlayerDownSmashState:BaseState<Player, PlayerEvent>
    {
       // private SmashComponent smashComp;
        private Vector2 smashDir;
        private const float DeadZone = 0.1f;

        public override void Enter()
        {
            base.Enter();
            //smashComp = owner.smashComponent;

            // // Prevent re-entering dash if already dashing
            // if (smashComp != null && smashComp.IsSmashing)
            //     return;

            // figure out direction: input or facing
            Vector2 input = owner.inputComponent.MoveVector;
            if (input.sqrMagnitude > DeadZone * DeadZone)
                smashDir = input.normalized;
            else
                smashDir = new Vector2(Mathf.Sign(owner.transform.localScale.x), 0f);

            // smashComp.OnSmashFinished += HandleSmashFinished;
            // smashComp.Smash(smashDir);

            owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.RUN);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
           // smashComp.OnSmashFinished -= HandleSmashFinished;
        }

        private void HandleSmashFinished()
        {
            
        }
    }
   
