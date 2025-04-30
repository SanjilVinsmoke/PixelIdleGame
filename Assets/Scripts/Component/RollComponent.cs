using UnityEngine;
using System;

public class RollComponent : MonoBehaviour
{
    [Header("Roll Settings")]
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollSpeed = 10f;
    [SerializeField] private float rollCooldown = 1.0f; // Time before another roll can start

    [Header("Dependencies")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private InputComponent inputComponent; // To get input direction
    [SerializeField] private Transform playerTransform; // To get facing direction if no input

    public bool IsRolling { get; private set; }
    public Vector2 RollDirection { get; private set; }

    private float rollStartTime;
    private float lastRollTime = -Mathf.Infinity; // Initialize to allow first roll immediately
    private const float DeadZone = 0.1f;

    public event Action OnRollStart;
    public event Action OnRollEnd;

    void Awake()
    {
        // Auto-assign components if not set in inspector
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (inputComponent == null) inputComponent = GetComponent<InputComponent>();
        if (playerTransform == null) playerTransform = transform; // Assuming component is on the player object
    }

    void FixedUpdate()
    {
        if (IsRolling)
        {
            HandleRollMovement();
            CheckRollCompletion();
        }
    }

    public bool CanRoll()
    {
        return !IsRolling && Time.time >= lastRollTime + rollCooldown;
    }

    public bool StartRoll()
    {
        if (!CanRoll())
        {
            return false; // Cannot start roll (already rolling or on cooldown)
        }

        IsRolling = true;
        rollStartTime = Time.time;
        lastRollTime = Time.time; // Start cooldown timer

        // Determine roll direction
        Vector2 input = inputComponent.MoveVector;
        if (input.sqrMagnitude > DeadZone * DeadZone)
        {
            // Prefer horizontal input direction for roll
            RollDirection = new Vector2(Mathf.Sign(input.x), 0f).normalized;
        }
        else
        {
            // If no input, roll in the direction the player is facing
            RollDirection = new Vector2(Mathf.Sign(playerTransform.localScale.x), 0f);
        }

        // Optional: Apply initial impulse or set velocity directly
        rb.linearVelocity = RollDirection * rollSpeed;

        OnRollStart?.Invoke(); // Notify listeners that roll started
        Debug.Log($"Roll Started. Direction: {RollDirection}");
        return true;
    }

    private void HandleRollMovement()
    {
        // Continuous velocity application during the roll
        // You might adjust this based on desired feel (e.g., decay speed)
        rb.linearVelocity = RollDirection * rollSpeed;
    }

    private void CheckRollCompletion()
    {
        if (Time.time >= rollStartTime + rollDuration)
        {
            EndRoll();
        }
    }

    public void EndRoll()
    {
        if (!IsRolling) return; // Avoid ending multiple times

        IsRolling = false;
        rb.linearVelocity = Vector2.zero; // Stop movement immediately after roll
        OnRollEnd?.Invoke(); // Notify listeners that roll ended
        Debug.Log("Roll Ended.");
    }

    // Optional: Allow external interruption of the roll
    public void InterruptRoll()
    {
        if (IsRolling)
        {
            EndRoll();
        }
    }
}