using UnityEngine;

namespace SpaceShip
{
    public class GameController : MonoBehaviour, IGameController
    {
        // --- Nuevo sistema de input ---
        private PlayerInputActions inputActions;
        public PlayerController playerController;

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.SpaceShipMinigame.Enable();

            // Movimiento
            inputActions.SpaceShipMinigame.Move.performed += ctx => playerController.moveInput = ctx.ReadValue<Vector2>();
            inputActions.SpaceShipMinigame.Move.canceled += ctx => playerController.moveInput = Vector2.zero;

            // Disparo
            inputActions.SpaceShipMinigame.Fire.performed += ctx => playerController.Fire();

            // Escudo
            inputActions.SpaceShipMinigame.Shield.performed += ctx => playerController.UseShields();

            // Cambio de armas
            inputActions.SpaceShipMinigame.ChangeWeapon1.performed += ctx => playerController.ChangeWeapon(0);
            inputActions.SpaceShipMinigame.ChangeWeapon2.performed += ctx => playerController.ChangeWeapon(1);
            inputActions.SpaceShipMinigame.ChangeWeapon3.performed += ctx => playerController.ChangeWeapon(2);
        }

        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;


            if (message.StartsWith("Tap"))
            {   
                playerController.Fire(); // Acción de disparo al recibir "Tap"
                return;
            }

            // --- Joystick:x,y ---
            if (message.StartsWith("Joystick:"))
            {
                string[] parts = message.Substring("Joystick:".Length).Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    Debug.Log("movement value:" + x + ", " + y);
                    playerController.moveInput = new Vector2(x, y);
                }
                return;
            }

            // --- Dpad:UP / DOWN / LEFT / RIGHT ---
            if (message.StartsWith("Dpad:"))
            {
                string dir = message.Substring("Dpad:".Length).ToUpper();
                switch (dir)
                {
                    case "UP":
                        playerController.moveInput = Vector2.up;
                        break;
                    case "DOWN":
                        playerController.moveInput = Vector2.down;
                        break;
                    case "LEFT":
                        playerController.moveInput = Vector2.left;
                        break;
                    case "RIGHT":
                        playerController.moveInput = Vector2.right;
                        break;
                    default:
                        Debug.LogWarning("Dirección Dpad desconocida: " + dir);
                        break;
                }
                return;
            }

            // --- Button:A / B / X / Y ---
            if (message.StartsWith("Button:"))
            {
                string button = message.Substring("Button:".Length).ToUpper();
                switch (button)
                {
                    case "A":
                        playerController.Fire();
                        break;
                    case "B":
                        playerController.UseShields();
                        break;
                    case "X":
                        playerController.ChangeWeapon(0);
                        break;
                    case "Y":
                        playerController.ChangeWeapon(1);
                        break;
                    default:
                        Debug.LogWarning("Botón desconocido: " + button);
                        break;
                }
                return;
            }

            Debug.LogWarning("Formato de mensaje no reconocido: " + message);
        }

    }
}
