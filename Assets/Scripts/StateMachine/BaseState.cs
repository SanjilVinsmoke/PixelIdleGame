using TMPro;
using UnityEngine;
using Utils;

public abstract class BaseState<T>
{
    protected T owner;
    protected StateMachine<T> stateMachine;
    private TextMeshPro stateLabel;
    
    // Inspector debug properties
    public string StateName => GetType().Name;
    public string StateDescription => GetStateDescription();
    public Color StateColor => GetStateColor();

    public virtual void Initialize(T owner, StateMachine<T> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;

        if (StateMachine<T>.DebugMode && owner is MonoBehaviour ownerBehaviour)
        {
            SetupDebugVisuals(ownerBehaviour);
        }
    }

    private void SetupDebugVisuals(MonoBehaviour ownerBehaviour)
    {
        // Create state label above player head if it doesn't exist
        if (stateLabel == null)
        {
            GameObject labelObj = new GameObject($"{ownerBehaviour.name}_StateLabel");
            labelObj.transform.SetParent(ownerBehaviour.transform);
            labelObj.transform.localPosition = Vector3.up * 2f; // Adjust height as needed
            
            stateLabel = labelObj.AddComponent<TextMeshPro>();
            stateLabel.alignment = TextAlignmentOptions.Center;
            stateLabel.fontSize = 3;
            stateLabel.enabled = StateMachine<T>.DebugMode;
        }
    }

    public virtual void Enter()
    {
        if (StateMachine<T>.DebugMode)
        {
            UpdateDebugVisuals();
        }
    }

    public virtual void Exit()
    {
        if (stateLabel != null)
        {
            stateLabel.text = "";
        }
    }

    public virtual void Update()
    {
        if (StateMachine<T>.DebugMode && stateLabel != null)
        {
            // Keep the label facing the camera
            stateLabel.transform.rotation = Quaternion.LookRotation(
                stateLabel.transform.position - Camera.main.transform.position
            );
        }
    }

    public virtual void FixedUpdate() { }

    private void UpdateDebugVisuals()
    {
        if (stateLabel != null)
        {
            stateLabel.text = StateName;
            stateLabel.color = StateColor;
        }
    }

    private string GetStateDescription()
    {
        var attributes = GetType().GetCustomAttributes(typeof(StateDescriptionAttribute), true);
        if (attributes.Length > 0)
        {
            return ((StateDescriptionAttribute)attributes[0]).description;
        }
        return "No description available";
    }

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