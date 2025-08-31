using UnityEngine;

namespace FishingGame
{
    public class GameController : MonoBehaviour , IGameController
    {
        PlayerInputActions inputActions;
        public PlayerController playerController;

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.FishingGame.Enable();

            // Reel
            inputActions.FishingGame.PressScreen.started += ctx => playerController.HookInput();
            inputActions.FishingGame.PressScreen.canceled += ctx => playerController.CancelHookInput();
            // Jump
            // inputActions.EndlessRunner.Jump.started += ctx => playerController.Jump();
            //inputActions.EndlessRunner.Jump.canceled += ctx => playerController.CancelJump();

            // Crounch
            // inputActions.EndlessRunner.Crounch.started += ctx => playerController.Crounch();
            // inputActions.EndlessRunner.Crounch.canceled += ctx => playerController.StandUp();
        }
        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (message.StartsWith("Time:"))
            {
                string[] parts = message.Substring("Joystick:".Length).Split(',');
                if (parts.Length == 1 &&
                    float.TryParse(parts[0], out float x))
                {
                    Debug.Log("Force value:" + x );
                    //playerController.moveInput = new Vector2(x);
                }
                return;
            }

            Debug.LogWarning("Formato de mensaje no reconocido: " + message);
        }
    }
}