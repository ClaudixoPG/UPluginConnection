using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


namespace GolfGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        private PlayerInputActions inputActions;
        public PlayerController playerController;

        private SceneManager sceneManager;


        [Header("Scene Flow")]
        [Tooltip("List of the scenes in order")]
        public List<string> sceneOrder = new List<string>();


        private int currentSceneIndex = 0;
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

            sceneManager = FindFirstObjectByType<SceneManager>();

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
                    case "FIRE":
                        //PluginActivity.Instance.UpdateControl(4); // Cambia a modo de fuerza
                        //do nothing
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
                if (parts.Length == 1 && float.TryParse(parts[0], out float x))
                {
                    // x ya está entre 0 y 1 (smartwatch lo envía normalizado)
                    playerController.isCharging = true;
                    playerController.SetNormalizedForce(x);
                }
                return;
            }

            // --- fuerza:x ---
            if (message.StartsWith("fuerzaRelease:"))
            {
                string[] parts = message.Substring("fuerzaRelease:".Length).Split(',');
                if (parts.Length == 1 && float.TryParse(parts[0], out float x))
                {
                    // último valor normalizado
                    playerController.SetNormalizedForce(x);
                    playerController.Release();
                    PluginActivity.Instance.UpdateControl(2); // Cambia a modo de dirección
                }
                return;
            }
        }


        //Manejo de escenas--------

        public void LoadNextGolfScene()
        {
            if (sceneManager == null || sceneOrder.Count == 0) return;

            currentSceneIndex++;
            if (currentSceneIndex >= sceneOrder.Count)
            {
                sceneManager.LoadScene("GolfMiniGame");
                return;
            }

            sceneManager.LoadScene(sceneOrder[currentSceneIndex]);
        }

        public void RestartCurrentScene()
        {
            if (sceneManager == null || sceneOrder.Count == 0) return;
            sceneManager.LoadScene(sceneOrder[currentSceneIndex]);
        }

        public void LoadSceneByName(string name)
        {
            if (sceneManager == null) return;

            if (sceneOrder.Contains(name))
            {
                currentSceneIndex = sceneOrder.IndexOf(name);
                sceneManager.LoadScene(name);
            }
        }

        public void Win()
        {
            LoadNextGolfScene();
        }
        public void Lose()
        {
            RestartCurrentScene();
        }
    }
}
