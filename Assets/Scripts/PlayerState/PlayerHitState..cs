

using Utils;

    [StateDescription("Player is hit")]
    [StateDebugColor(StateDebugColorAttribute.UnityColor.Yellow)]
    public class PlayerHitState: BaseState<Player, PlayerEvent>
    {
        public override void Enter()
        {
            base.Enter();
            
            Hit();
            
        }
        
        private void Hit()
        {
            if (owner.healthComponent != null)
            {
                DOTweenHelper.Shake(owner.transform);
                owner.healthComponent.TakeDamage(10);
            }
        }
    }
