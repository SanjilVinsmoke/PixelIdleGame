using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    private T owner;
    private Dictionary<Type, BaseState<T>> states = new Dictionary<Type, BaseState<T>>();
    private BaseState<T> currentState;
    
    // Add debug mode flag
    public static bool DebugMode { get; set; } = false;

    public StateMachine(T owner)
    {
        this.owner = owner;
    }

    // Property to get current state name
    public string CurrentStateName => currentState?.GetType().Name ?? "No State";

    public void AddState(BaseState<T> state)
    {
        states[state.GetType()] = state;
        state.Initialize(owner, this);
        
        if (DebugMode)
        {
            Debug.Log($"[StateMachine] Added state: {state.GetType().Name}");
        }
    }

    public void SetInitialState<TState>() where TState : BaseState<T>
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

    public void ChangeState<TState>() where TState : BaseState<T>
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

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    // Helper method to get all registered states
    public List<string> GetRegisteredStates()
    {
        List<string> stateNames = new List<string>();
        foreach (var state in states.Keys)
        {
            stateNames.Add(state.Name);
        }
        return stateNames;
    }

    // Optional: Method to toggle debug mode
    public static void ToggleDebugMode()
    {
        DebugMode = !DebugMode;
        Debug.Log($"[StateMachine] Debug mode: {(DebugMode ? "Enabled" : "Disabled")}");
    }
}