using System.Collections;
using UnityEngine;

namespace EndlessRunner
{
    public class GameController : MonoBehaviour, IGameController
    {
        private PlayerInputActions inputActions;

        [Header("Scene References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private Spawner spawner;

        [Header("Restart Flow")]
        [SerializeField] private float loseScreenDuration = 2f;
        [SerializeField] private int countdownStart = 3;
        [SerializeField] private float countdownStepDuration = 1f;

        [Header("Difficulty Progression")]
        [SerializeField] private float baseObstacleSpeed = 10f;
        [SerializeField] private float maxObstacleSpeed = 22f;
        [SerializeField] private float speedIncreaseInterval = 5f;
        [SerializeField] private float speedIncreaseAmount = 1f;

        public static GameController Instance;
        public static bool IsGameOver = false;

        public float currentScore = 0f;
        public float CurrentObstacleSpeed { get; private set; }

        private Vector3 _playerStartPosition;
        private Vector3 _playerStartScale;
        private float _speedIncreaseTimer;

        private bool IsGameplayActive =>
            !IsGameOver && MinigameContext.IsMeasurementActive;

        private void Awake()
        {
            if (Instance == null) Instance = this;

            inputActions = new PlayerInputActions();
            inputActions.EndlessRunner.Enable();

            if (playerController != null)
            {
                _playerStartPosition = playerController.transform.position;
                _playerStartScale = playerController.transform.localScale;
            }

            inputActions.EndlessRunner.Jump.started += ctx =>
            {
                if (IsGameplayActive && playerController != null)
                    playerController.Jump();
            };

            inputActions.EndlessRunner.Jump.canceled += ctx =>
            {
                if (IsGameplayActive && playerController != null)
                    playerController.CancelJump();
            };

            inputActions.EndlessRunner.Crounch.started += ctx =>
            {
                if (IsGameplayActive && playerController != null)
                    playerController.Crounch();
            };

            inputActions.EndlessRunner.Crounch.canceled += ctx =>
            {
                if (IsGameplayActive && playerController != null)
                    playerController.StandUp();
            };
        }

        private void Start()
        {
            ResetGameState();
        }

        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();

        public void HandleMessage(string message)
        {
            if (!IsGameplayActive) return;
            if (string.IsNullOrEmpty(message)) return;

            if (message.StartsWith("Tap"))
            {
                if (playerController != null)
                    playerController.Jump();
            }
        }

        private void Update()
        {
            if (!IsGameplayActive) return;

            currentScore += Time.deltaTime;
            UpdateDifficulty();

            if (uiManager != null)
            {
                uiManager.UpdateScore(currentScore);
            }
        }

        private void UpdateDifficulty()
        {
            _speedIncreaseTimer += Time.deltaTime;

            if (_speedIncreaseTimer >= speedIncreaseInterval)
            {
                _speedIncreaseTimer = 0f;
                CurrentObstacleSpeed = Mathf.Min(
                    CurrentObstacleSpeed + speedIncreaseAmount,
                    maxObstacleSpeed
                );
            }
        }

        public void GameOver()
        {
            if (IsGameOver) return;

            IsGameOver = true;
            inputActions.Disable();

            if (uiManager != null)
            {
                uiManager.ShowLoseScreen(currentScore);
            }

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.gameObject.SetActive(false);
            }

            StartCoroutine(RestartFlow());
        }

        private IEnumerator RestartFlow()
        {
            yield return new WaitForSeconds(loseScreenDuration);

            bool countdownDone = false;

            TransitionOverlayUI.Instance.ShowCountdown(
                countdownStart,
                countdownStepDuration,
                () => countdownDone = true
            );

            yield return new WaitUntil(() => countdownDone);

            RestartGame();
        }

        private void RestartGame()
        {
            ClearObstacles();
            ResetGameState();

            if (playerController != null)
            {
                playerController.transform.position = _playerStartPosition;
                playerController.transform.localScale = _playerStartScale;
                playerController.gameObject.SetActive(true);
                playerController.ResetState();
                playerController.SetGameplayEnabled(true);
            }

            if (spawner != null)
            {
                spawner.ResetSpawner();
            }

            if (uiManager != null)
            {
                uiManager.HideLoseScreen();
                uiManager.UpdateScore(currentScore);
            }

            inputActions.Enable();
        }

        private void ResetGameState()
        {
            IsGameOver = false;
            currentScore = 0f;
            CurrentObstacleSpeed = baseObstacleSpeed;
            _speedIncreaseTimer = 0f;

            if (uiManager != null)
            {
                uiManager.HideLoseScreen();
                uiManager.UpdateScore(currentScore);
            }
        }

        private void ClearObstacles()
        {
            var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (var obstacle in obstacles)
            {
                Destroy(obstacle);
            }
        }
    }
}