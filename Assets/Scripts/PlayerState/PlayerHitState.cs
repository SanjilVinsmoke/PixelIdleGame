using Component;
using Constant;
using UnityEngine;

// Corrected: Inherit from BaseState<Player, PlayerEvent>
public class PlayerHitState : BaseState<Player, PlayerEvent>
{
    private Vector2 hitDirection;
    private float hitStartTime;

    private HitComponent hitComponent;

    // Added: Initialize method override


    public override void Enter()
    {
        base.Enter();
        hitComponent = owner.hitComponent;
       

        owner.animationComponent.PlayAnimation(AnimationName.PlayerAnimationNames.HIT);
        hitComponent.TakeHit(owner.LastHitDirection);
        
    }
    
    public override void Update()
    {
        base.Update();
        if (!hitComponent.IsHit)
        {
           stateMachine.ProcessEvent(PlayerEvent.Idle);
        }

   
    }

    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // Knockback movement is handled by HitComponent's coroutine
    }

    public override void Exit()
    {
        base.Exit();
        // Ensure hit effects are cleaned up if state is exited prematurely (e.g., death)
        // HitComponent might handle this internally when IsHit becomes false
        // hitComponent?.CancelHitEffects();
    }

    
}