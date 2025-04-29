using Constant;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

[StateDebugColor(StateDebugColorAttribute.UnityColor.Orange)]
[StateDescription("Handles a quick dash, then returns to Move/Idle.")]
public class PlayerDashState : BaseState<Player, PlayerEvent>
{
    private DashComponent dashComp;
    private Vector2       dashDir;
    private const float   DeadZone = 0.1f;
    private bool dashFinishedHandled = false;

    public override void Enter()
    {
        base.Enter();
        dashComp = owner.dashComponent;

        // Prevent re-entering dash if already dashing
        if (dashComp != null && dashComp.IsDashing)
            return;

        // figure out direction: input or facing
        Vector2 input = owner.inputComponent.MoveVector;
        if (input.sqrMagnitude > DeadZone * DeadZone)
            dashDir = input.normalized;
        else
            dashDir = new Vector2(Mathf.Sign(owner.transform.localScale.x), 0f);

        dashComp.OnDashFinished += HandleDashFinished;
        dashComp.Dash(dashDir);

        owner.animationComponent?.PlayAnimation(AnimationName.PlayerAnimationNames.RUN);
        dashFinishedHandled = false;
    }

    public override void Update()
    {
        base.Update();
        // Safety: If dash ends but event missed, exit state
        if (!dashComp.IsDashing && !dashFinishedHandled)
        {
            HandleDashFinished();
        }
    }

    public override void Exit()
    {
        base.Exit();
        dashComp.OnDashFinished -= HandleDashFinished;
    }

    private void HandleDashFinished()
    {
        if (dashFinishedHandled) return;
        dashFinishedHandled = true;
        dashComp.OnDashFinished -= HandleDashFinished;
        if (Mathf.Abs(owner.inputComponent.MoveVector.x) > DeadZone)
            stateMachine.ProcessEvent(PlayerEvent.Move);
        else
            stateMachine.ProcessEvent(PlayerEvent.Idle);
    }
}