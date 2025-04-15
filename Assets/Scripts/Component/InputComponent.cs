using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Component
{


    public class InputComponent : MonoBehaviour, PlayerInputAction.IGameplayActions
    {
        private PlayerInputAction inputActions;

        // Movement values
        private Vector2 moveInput;
        public float MoveX => moveInput.x;
        public float MoveY => moveInput.y;
        public Vector2 MoveVector => moveInput;

        // Look values
        private Vector2 lookInput;
        public Vector2 LookDirection => lookInput;

        // Events
        public event Action OnFirePerformed;
        public event Action OnJumpPerformed;

        private void Awake()
        {
            // Create the input actions instance
            inputActions = new PlayerInputAction();

            // Register this component to receive callbacks
            inputActions.gameplay.AddCallbacks(this);
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
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnJumpPerformed?.Invoke();
            }
        }
    }
}