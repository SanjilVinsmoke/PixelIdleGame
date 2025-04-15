using System;

/// <summary>
/// An abstract state that supports nested (hierarchical) substates.
/// </summary>
public abstract class `erarchicalState<T, TEvent> : BaseState<T, TEvent> where TEvent : Enum
{
    protected BaseState<T, TEvent> currentSubState;

    /// <summary>
    /// Sets the active substate. Exits any previously active substate.
    /// </summary>
    public virtual void SetSubState(BaseState<T, TEvent> newSubState)
    {
        // Exit current substate if one exists.
        currentSubState?.Exit();

        // Initialize and enter the new substate.
        currentSubState = newSubState;
        currentSubState.Initialize(owner, stateMachine);
        currentSubState.Enter();
    }

    /// <summary>
    /// Override Enter to optionally enter a default substate.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        // Optionally, you can set a default substate here.
        currentSubState?.Enter();
    }

    /// <summary>
    /// Ensure that update calls are cascaded to the active substate.
    /// </summary>
    public override void Update()
    {
        base.Update();
        currentSubState?.Update();
    }

    /// <summary>
    /// Ensure that FixedUpdate calls are cascaded to the active substate.
    /// </summary>
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        currentSubState?.FixedUpdate();
    }

    /// <summary>
    /// Override Exit to exit the substate first.
    /// </summary>
    public override void Exit()
    {
        currentSubState?.Exit();
        base.Exit();
    }
}