using UnityEngine;

namespace EndlessRunner
{
    public class GameController : MonoBehaviour, IGameController
    {
        PlayerInputActions inputActions;
        public PlayerController playerController;

        #region Singleton

        public static GameController Instance;

        #endregion

        public static bool IsGameOver = false;
        public float currentScore = 0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;

            inputActions = new PlayerInputActions();
            inputActions.EndlessRunner.Enable();

            // Jump
            inputActions.EndlessRunner.Jump.started += ctx => playerController.Jump();
            inputActions.EndlessRunner.Jump.canceled += ctx => playerController.CancelJump();

            // Crounch
            inputActions.EndlessRunner.Crounch.started += ctx => playerController.Crounch();
            inputActions.EndlessRunner.Crounch.canceled += ctx => playerController.StandUp();
        }
        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();
        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (message.StartsWith("Tap"))
            {
                playerController.Jump(); // Acci�n de disparo al recibir "Tap"
                return;
            }

            Debug.LogWarning("Formato de mensaje no reconocido: " + message);
        }

        private void Update()
        {
            if (IsGameOver) return;
            currentScore += Time.deltaTime;
        }

        public void GameOver()
        {
            IsGameOver = true;
            inputActions.Disable();
            // Show Game Over UI or any other logic
            Debug.Log("Game Over! Final Score: " + Mathf.RoundToInt(currentScore));
        }


    }
}

