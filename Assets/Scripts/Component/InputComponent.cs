using UnityEngine;
using UnityEngine.InputSystem;
using System;


    public class InputComponent : MonoBehaviour, PlayerInputAction.IGameplayActions
    {
        private PlayerInputAction inputActions;

        // Movement values
        private Vector2 moveInput;
        public float MoveX => moveInput.x;
        public float MoveY => moveInput.y;
        public Vector2 MoveVector => moveInput;

        public Vector2 lastMoveDirection;

        // Look values
        private Vector2 lookInput;
        public Vector2 LookDirection => lookInput;

        // Input Events
        public event Action OnAttackPerformed;
        public event Action OnJumpPerformed;
        public event Action OnInteractPerformed;
        public event Action OnDashPerformed; // Added Dash event
        
        public event Action OnJumpCanceled;
        public event Action<InputAction.CallbackContext> OnMovePerformed;
        public event Action<InputAction.CallbackContext> OnLookPerformed;


        private void Awake()
        {
            // Create the input actions instance
            inputActions = new PlayerInputAction();

            // Register this component to receive callbacks
            inputActions.gameplay.SetCallbacks(this);

            // Manually subscribe to dash action if needed (assuming it's named "Dash")
           // inputActions.gameplay.Dash.performed += OnDash; // Added Dash subscription
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
            // Unsubscribe from dash action
            //inputActions.gameplay.Dash.performed -= OnDash; // Added Dash unsubscription
        }

        // IGameplayActions implementation
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnAttackPerformed?.Invoke();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {

            moveInput = context.ReadValue<Vector2>();
            OnMovePerformed?.Invoke(context);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
            OnLookPerformed?.Invoke(context);
        }

        public void OnJump(InputAction.CallbackContext context)
        {

            if (context.performed)
                OnJumpPerformed?.Invoke();
            else if (context.canceled)
                OnJumpCanceled?.Invoke();  
        }

        // Callback for the Dash action
        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnDashPerformed?.Invoke();
            }
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteractPerformed?.Invoke();
            }
        }
        
        public void DisableInput()
        {
            inputActions.Disable();
        }
        public void EnableInput()
        {
            inputActions.Enable();
        }
  
        
        
    }