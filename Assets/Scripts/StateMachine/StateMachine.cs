using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T, TEvent> : IDisposable where TEvent : Enum
{
    private T owner;
    private Dictionary<Type, BaseState<T, TEvent>> states = new Dictionary<Type, BaseState<T, TEvent>>();
    private BaseState<T, TEvent> currentState;
    private Dictionary<TEvent, Action> eventActions = new Dictionary<TEvent, Action>();

    // Add this property to expose the current state
    public BaseState<T, TEvent> CurrentState => currentState;

    public static bool DebugMode { get; set; } = false;

    public StateMachine(T owner, Action<StateMachine<T, TEvent>> configureEventMappings = null)
    {
        this.owner = owner;
        configureEventMappings?.Invoke(this);
    }

    public string CurrentStateName => currentState?.GetType().Name ?? "No State";

    public void AddState(BaseState<T, TEvent> state)
    {
        states[state.GetType()] = state;
        state.Initialize(owner, this);
        if (DebugMode)
        {
            Debug.Log($"[StateMachine] Added state: {state.GetType().Name}");
        }
    }

    public void SetInitialState<TState>() where TState : BaseState<T, TEvent>
    {
        Type stateType = typeof(TState);
        if (states.ContainsKey(stateType))
        {
            currentState = states[stateType];
            currentState.Enter();
            if (DebugMode)
            {
                Debug.Log($"[StateMachine] Set initial state to: {CurrentStateName}");
            }
        }
        else
        {
            Debug.LogError($"State {stateType.Name} not found in state machine!");
        }
    }

    public void ChangeState<TState>() where TState : BaseState<T, TEvent>
    {
        Type stateType = typeof(TState);
        if (states.ContainsKey(stateType))
        {
            if (DebugMode)
            {
                Debug.Log($"[StateMachine] Changing state from {CurrentStateName} to {stateType.Name}");
            }
            currentState?.Exit();
            currentState = states[stateType];
            currentState.Enter();
        }
        else
        {
            Debug.LogError($"State {stateType.Name} not found in state machine!");
        }
    }

    public void Update() => currentState?.Update();
    public void FixedUpdate() => currentState?.FixedUpdate();

    public void AddEventMapping(TEvent stateEvent, Action action)
    {
        eventActions[stateEvent] = action;
    }

    public void ProcessEvent(TEvent stateEvent)
    {
        if (eventActions.TryGetValue(stateEvent, out Action action))
        {
            action?.Invoke();
        }
        else
        {
            Debug.LogWarning($"No action mapped for event: {stateEvent}");
        }
    }

    public void Dispose()
    {
        foreach (var state in states.Values)
        {
            if (state is IDisposable disposableState)
            {
                disposableState.Dispose();
            }
        }
        states.Clear();
        eventActions.Clear();
        currentState = null;
    }
}
