using System.Collections;
using UnityEngine;

namespace SpaceShip
{
    public class GameController : MonoBehaviour, IGameController
    {
        private PlayerInputActions inputActions;

        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UIManager uiManager;

        [Header("Restart Flow")]
        [SerializeField] private float loseScreenDuration = 2f;
        [SerializeField] private int countdownStart = 3;
        [SerializeField] private float countdownStepDuration = 1f;

        public enum GameState
        {
            Waiting,
            Playing,
            Results
        }

        public GameState CurrentState { get; private set; } = GameState.Waiting;

        public static GameController Instance { get; private set; }

        private bool IsGameplayActive =>
            CurrentState == GameState.Playing &&
            MinigameContext.IsMeasurementActive;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            inputActions = new PlayerInputActions();
            inputActions.SpaceShipMinigame.Enable();

            inputActions.SpaceShipMinigame.Move.performed += ctx =>
            {
                if (IsGameplayActive && playerController != null)
                    playerController.SetMoveInput(ctx.ReadValue<Vector2>());
            };

            inputActions.SpaceShipMinigame.Move.canceled += ctx =>
            {
                if (playerController != null)
                    playerController.SetMoveInput(Vector2.zero);
            };
        }

        private void Start()
        {
            if (uiManager != null)
            {
                uiManager.HideLoseScreen();
            }

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
            }

            if (gameManager != null)
            {
                gameManager.SetGameplayEnabled(false);
                gameManager.ResetRun();
            }
        }

        private void OnEnable() => inputActions.Enable();

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void Update()
        {
            if (CurrentState == GameState.Waiting && MinigameContext.IsMeasurementActive)
            {
                StartRun();
            }

            if (CurrentState == GameState.Playing && !MinigameContext.IsMeasurementActive)
            {
                StopRunSilently();
            }
        }

        public void HandleMessage(string message)
        {
            if (!IsGameplayActive || playerController == null) return;
            if (string.IsNullOrWhiteSpace(message)) return;

            if (message.StartsWith("Joystick:"))
            {
                string[] parts = message.Substring("Joystick:".Length).Split(',');

                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    playerController.SetMoveInput(new Vector2(x, y));
                }

                return;
            }
        }

        private void StartRun()
        {
            CurrentState = GameState.Playing;

            if (uiManager != null)
            {
                uiManager.HideLoseScreen();
            }

            if (gameManager != null)
            {
                gameManager.ResetRun();
                gameManager.SetGameplayEnabled(true);
            }

            if (playerController != null)
            {
                playerController.ResetShip();
                playerController.SetGameplayEnabled(true);
                playerController.SetMoveInput(Vector2.zero);
            }
        }

        private void StopRunSilently()
        {
            CurrentState = GameState.Waiting;

            if (gameManager != null)
            {
                gameManager.SetGameplayEnabled(false);
            }

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.SetMoveInput(Vector2.zero);
            }
        }

        public void OnPlayerDied()
        {
            if (CurrentState != GameState.Playing)
                return;

            CurrentState = GameState.Results;

            if (gameManager != null)
            {
                gameManager.SetGameplayEnabled(false);
            }

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.SetMoveInput(Vector2.zero);
            }

            if (uiManager != null && gameManager != null && playerController != null)
            {
                uiManager.ShowLoseScreen(
                    gameManager.Score,
                    gameManager.ElapsedTime,
                    playerController.Lives,
                    playerController.CurrentWeaponName
                );
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

            ClearRuntimeObjects();

            CurrentState = GameState.Waiting;
        }

        private void ClearRuntimeObjects()
        {
            foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                Destroy(enemy.gameObject);
            }

            foreach (var bullet in FindObjectsByType<Bullet>(FindObjectsSortMode.None))
            {
                Destroy(bullet.gameObject);
            }

            foreach (var powerUp in FindObjectsByType<WeaponPowerUp>(FindObjectsSortMode.None))
            {
                Destroy(powerUp.gameObject);
            }
        }
    }
}