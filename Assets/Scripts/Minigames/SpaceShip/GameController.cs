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

            // Movimiento
            inputActions.Player.Move.performed += ctx => playerController.moveInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += ctx => playerController.moveInput = Vector2.zero;

            // Disparo
            inputActions.Player.Fire.performed += ctx => playerController.Fire();

            // Escudo
            inputActions.Player.Shield.performed += ctx => playerController.UseShields();

            // Cambio de armas
            inputActions.Player.ChangeWeapon1.performed += ctx => playerController.ChangeWeapon(0);
            inputActions.Player.ChangeWeapon2.performed += ctx => playerController.ChangeWeapon(1);
            inputActions.Player.ChangeWeapon3.performed += ctx => playerController.ChangeWeapon(2);
        }

        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();

        public void HandleMessage(string message)
        {
            if(message.Equals("arriba"))
            {
                playerController.moveInput = Vector2.up;
                return;
            }
            if(message.Equals("abajo"))
            {
                playerController.moveInput = Vector2.down;
                return;
            }
            if(message.Equals("izquierda"))
            {
                playerController.moveInput = Vector2.left;
                return;
            }
            if(message.Equals("derecha"))
            {
                playerController.moveInput = Vector2.right;
                return;
            }
            if(message.Equals("Shoot"))
            {
                playerController.Fire();
                return;
            }
            return;

            //NEXT STEP: Manejar mensajes de joystick, dpad y botones
            //reviar on release y on press


            Debug.Log("Game1: mensaje recibido: " + message);

            if (string.IsNullOrEmpty(message))
                return;

            // --- Joystick:x,y ---
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
