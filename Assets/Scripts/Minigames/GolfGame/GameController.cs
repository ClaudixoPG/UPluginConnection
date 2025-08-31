using NUnit.Framework;
using RythmGame;
using UnityEngine;



namespace GolfGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        private PlayerInputActions inputActions;
        public PlayerController playerController;

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.GolfMiniGame.Enable();

            //Movimiento

            //inputActions.GolfMiniGame.LaunchForce.performed += ctx => playerController.DragChange();
            //inputActions.GolfMiniGame.LaunchForce.canceled += ctx => playerController.DragRelease();

            inputActions.GolfMiniGame.LaunchForce.started += ctx => playerController.isCharging = true;
            inputActions.GolfMiniGame.LaunchForce.canceled += ctx => playerController.Release();

            //Turn left
            inputActions.GolfMiniGame.LeftDirection.started += ctx => playerController.isTurningLeft = true;
            inputActions.GolfMiniGame.LeftDirection.canceled += ctx => playerController.isTurningLeft = false;

            //Turn Right
            inputActions.GolfMiniGame.RightDirection.started += ctx => playerController.isTurningRight = true;
            inputActions.GolfMiniGame.RightDirection.canceled += ctx => playerController.isTurningRight = false;
          

        }

        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();
        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            #region

            if (message.StartsWith("Dpad:"))
            {
                string dir = message.Substring("Dpad:".Length).ToUpper();
                switch (dir)
                {
                    case "LEFT":
                        playerController.isTurningLeft = true;
                        break;
                    case "RIGHT":
                        playerController.isTurningRight = true;
                        break;
                    case "FIRE":
                        PluginActivity.Instance.UpdateControl(4); // Cambia a modo de fuerza
                        break;

                    default:
                        Debug.LogWarning("Dirección Dpad desconocida: " + dir);
                        break;
                }
                return;
            }

            if (message.StartsWith("DpadRelease:"))
            {
                string dir = message.Substring("DpadRelease:".Length).ToUpper();
                switch (dir)
                {
                    case "LEFT":
                        playerController.isTurningLeft = false;
                        break;
                   
                    case "RIGHT":
                        playerController.isTurningRight = false;
                        break;
                   
                    default:
                        Debug.LogWarning("Dirección Dpad desconocida: " + dir);
                        break;
                }
                return;
            }

            #endregion

            // --- fuerza:x ---
            if (message.StartsWith("fuerza:"))
            {
                string[] parts = message.Substring("fuerza:".Length).Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x))
                {
                    Debug.Log("fuerza value:" + x);
                    playerController.isCharging = true;
                }
                return;
            }

            // --- fuerza:x ---
            if (message.StartsWith("fuerzaRelease:"))
            {
                string[] parts = message.Substring("fuerzaRelease:".Length).Split(',');
                if (parts.Length == 1 &&
                    float.TryParse(parts[0], out float x))
                {
                    PluginActivity.Instance.UpdateControl(1); // Cambia a modo de dirección
                    playerController.Release();
                }
                return;
            }


        }
    }
}
