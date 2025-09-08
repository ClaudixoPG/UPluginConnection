using MinigameSystem;
using UnityEngine;

namespace ReignsGame
{
    public class GameController : MinigameHandler, IGameController
    {
        private PlayerInputActions inputActions;
        public PlayerController playerController;

        private void Awake()
        {
            //inputActions = new PlayerInputActions();
            //inputActions.ReignsGame.Enable();

            //// --- InputActions (para test en PC / Unity Input System) ---
            //inputActions.ReignsGame.Move.performed += ctx => playerController.moveInput = ctx.ReadValue<Vector2>();

            ////inputActions.ReignsGame.Move.canceled += ctx => playerController.moveInput = Vector2.zero;
            //inputActions.ReignsGame.Move.canceled += ctx => playerController.OnJoystickRelease(playerController.moveInput);
        }

        private void OnEnable() => inputActions?.Enable();
        private void OnDisable() => inputActions?.Disable();

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // --- Movimiento continuo ---
            if (message.StartsWith("Joystick:"))
            {
                string[] parts = message.Substring("Joystick:".Length).Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    playerController.moveInput = new Vector2(x, y);
                }
                return;
            }

            // --- Release (decisión final, usa eje Y) ---
            if (message.StartsWith("JoystickRelease:"))
            {
                string[] parts = message.Substring("JoystickRelease:".Length).Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    Vector2 releaseInput = new Vector2(x, y);
                    playerController.OnJoystickRelease(releaseInput);
                }
                return;
            }

            Debug.LogWarning("Formato de mensaje no reconocido: " + message);
        }

        protected override void OnStartGame()
        {
            inputActions = new PlayerInputActions();
            inputActions.ReignsGame.Enable();

            // --- InputActions (para test en PC / Unity Input System) ---
            inputActions.ReignsGame.Move.performed += ctx => playerController.moveInput = ctx.ReadValue<Vector2>();

            //inputActions.ReignsGame.Move.canceled += ctx => playerController.moveInput = Vector2.zero;
            inputActions.ReignsGame.Move.canceled += ctx => playerController.OnJoystickRelease(playerController.moveInput);
        }

        protected override void UpdateGame()
        {
            
        }
    }
}
