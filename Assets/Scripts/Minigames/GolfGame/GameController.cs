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

        }
    }
}
