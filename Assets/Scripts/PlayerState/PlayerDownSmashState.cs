
using UnityEngine;
using UnityEngine.Events;
using Component;
using Constant;
using Utils;

[StateDescription("Player is performing a down smash attack")]
[StateDebugColor(StateDebugColorAttribute.UnityColor.Magenta)]
public class PlayerDownSmashState : BaseState<Player, PlayerEvent>
{
    private SmashComponent smashComp;
    private const float DeadZone = 0.1f;
    private bool hasEndedSmash = false;

    public override void Enter()
    {
        base.Enter();
        smashComp = owner.smashComponent;
        hasEndedSmash = false;
        
        if (smashComp == null) return;
        
        // Force cancel any existing smash to ensure clean state
        smashComp.CancelSmash();
        
        Vector2 input = owner.inputComponent.MoveVector;
        Vector2 smashDir = input.sqrMagnitude > DeadZone * DeadZone
            ? input.normalized
            : new Vector2(Mathf.Sign(owner.transform.localScale.x), 0f);

        // Remove any existing listeners first to prevent duplicates
        smashComp.onSmashStart.RemoveListener(OnSmashStart);
        smashComp.onSmashImpact.RemoveListener(OnSmashImpact);
        smashComp.onSmashEnd.RemoveListener(OnSmashEnd);
        
        // Add listeners
        smashComp.onSmashStart.AddListener(OnSmashStart);
        smashComp.onSmashImpact.AddListener(OnSmashImpact);
        smashComp.onSmashEnd.AddListener(OnSmashEnd);

        smashComp.DownSmash();
    }

    public override void Exit()
    {
        base.Exit();
        if (smashComp != null)
        {
            // If we're exiting the state without properly ending the smash, cancel it
            if (!hasEndedSmash)
            {
                smashComp.CancelSmash();
            }
            
            smashComp.onSmashStart.RemoveListener(OnSmashStart);
            smashComp.onSmashImpact.RemoveListener(OnSmashImpact);
            smashComp.onSmashEnd.RemoveListener(OnSmashEnd);
        }
    }

    public override void Update()
    {
        base.Update();
        
        // Safety check - if we're not smashing anymore but still in this state, force transition
        if (smashComp != null && !smashComp.IsSmashing && !hasEndedSmash)
        {
            Debug.Log("Forcing state transition - smash ended without callback");
            OnSmashEnd();
        }
    }

    private void OnSmashStart()
    {
        owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.FALL);
    }

    private void OnSmashImpact()
    { 
       
        owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.LANDED);
    }

    private void OnSmashEnd()
    {
        if (hasEndedSmash) return; // Prevent multiple calls
        
        
        hasEndedSmash = true;
        
        owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.LANDED);
       
        
        // Only process one state transition
        if(owner.jumpComponent.isGrounded)
        {
            // If there's movement input, go to move state, otherwise idle
            if(owner.inputComponent.MoveVector.magnitude > DeadZone)
            {
                stateMachine.ProcessEvent(PlayerEvent.Move);
            }
            else
            {
                stateMachine.ProcessEvent(PlayerEvent.Idle);
            }
        }
        else
        {
            // If in the air, transition to fall state
            stateMachine.ProcessEvent(PlayerEvent.Fall);
        }
    }
}