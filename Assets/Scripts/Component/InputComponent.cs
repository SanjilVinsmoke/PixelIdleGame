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

        // Events
        public event Action OnFirePerformed;
        public event Action OnJumpPerformed;

        public event Action OnInteractPerformed;
        public event Action<InputAction.CallbackContext> OnMovePerformed;

        public event Action<InputAction.CallbackContext> OnLookPerformed;


        private void Awake()
        {
            // Create the input actions instance
            inputActions = new PlayerInputAction();

            // Register this component to receive callbacks
            inputActions.gameplay.SetCallbacks(this);
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        // IGameplayActions implementation
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnFirePerformed?.Invoke();
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
            {
                OnJumpPerformed?.Invoke();
            }
        }
    }