using TMPro;
using UnityEngine;
using Utils;
using System;

/// <summary>
/// Base class for all states in the state machine.
/// </summary>
public abstract class BaseState<T, TEvent> where TEvent : Enum
{
    protected T owner;
    protected StateMachine<T, TEvent> stateMachine;
    private TextMeshPro stateLabel;

    // Inspector debug properties
    public string StateName => GetType().Name;
    public string StateDescription => GetStateDescription();
    public Color StateColor => GetStateColor();

    /// <summary>
    /// Initializes the state with the owner and state machine.
    /// </summary>
    public virtual void Initialize(T owner, StateMachine<T, TEvent> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;

        if (StateMachine<T, TEvent>.DebugMode && owner is MonoBehaviour ownerBehaviour)
        {
            SetupDebugVisuals(ownerBehaviour);
        }
    }

    /// <summary>
    /// Sets up the debug state label above the owner's head.
    /// </summary>
    private void SetupDebugVisuals(MonoBehaviour ownerBehaviour)
    {
        if (stateLabel == null)
        {
            GameObject labelObj = new GameObject($"{ownerBehaviour.name}_StateLabel");
            labelObj.transform.SetParent(ownerBehaviour.transform);
            labelObj.transform.localPosition = Vector3.up * 2f; // Adjust height as needed
            
            stateLabel = labelObj.AddComponent<TextMeshPro>();
            stateLabel.alignment = TextAlignmentOptions.Center;
            stateLabel.fontSize = 3;
            stateLabel.enabled = StateMachine<T, TEvent>.DebugMode;
        }
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public virtual void Enter()
    {
        if (StateMachine<T, TEvent>.DebugMode)
        {
            UpdateDebugVisuals();
        }
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public virtual void Exit()
    {
        if (stateLabel != null)
        {
            stateLabel.text = "";
        }
    }

    /// <summary>
    /// Called every frame while the state is active.
    /// </summary>
    public virtual void Update()
    {
        if (StateMachine<T, TEvent>.DebugMode && stateLabel != null)
        {
            if (Camera.main != null) // Avoid potential NullReferenceException
            {
                stateLabel.transform.rotation = Quaternion.LookRotation(
                    stateLabel.transform.position - Camera.main.transform.position
                );
            }
        }
    }

    /// <summary>
    /// Called during FixedUpdate for physics-related logic.
    /// </summary>
    public virtual void FixedUpdate() { }

    /// <summary>
    /// Updates the debug label with the state name and color.
    /// </summary>
    private void UpdateDebugVisuals()
    {
        if (stateLabel != null)
        {
            stateLabel.text = StateName;
            stateLabel.color = StateColor;
        }
    }

    /// <summary>
    /// Retrieves the state description from the StateDescriptionAttribute.
    /// </summary>
    private string GetStateDescription()
    {
        var attributes = GetType().GetCustomAttributes(typeof(StateDescriptionAttribute), true);
        if (attributes.Length > 0)
        {
            return ((StateDescriptionAttribute)attributes[0]).description;
        }
        return "No description available";
    }

    /// <summary>
    /// Retrieves the state color from the StateDebugColorAttribute.
    /// </summary>
    private Color GetStateColor()
    {
        var attributes = GetType().GetCustomAttributes(typeof(StateDebugColorAttribute), true);
        if (attributes.Length > 0)
        {
            return ((StateDebugColorAttribute)attributes[0]).Color;
        }
        return Color.white;
    }
}
