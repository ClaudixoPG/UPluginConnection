using System.Collections;
using TMPro;
using UnityEngine;

namespace FishingGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        private PlayerInputActions inputActions;

        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private FishingUIManager uiManager;

        [Header("Loop")]
        [SerializeField] private float resultsScreenDuration = 2f;
        [SerializeField] private int countdownStart = 3;
        [SerializeField] private float countdownStepDuration = 1f;

        public enum GameState
        {
            Waiting,
            Playing,
            Results
        }

        public GameState CurrentState { get; private set; } = GameState.Waiting;

        public int fishCaught;
        public int fishMissed;

        private Coroutine restartLoopRoutine;

        private bool IsGameplayActive =>
            CurrentState == GameState.Playing &&
            MinigameContext.IsMeasurementActive;

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.FishingGame.Enable();

            inputActions.FishingGame.PressScreen.started += ctx =>
            {
                if (IsGameplayActive && playerController != null)
                    playerController.SetHoldInput(1f);
            };

            inputActions.FishingGame.PressScreen.canceled += ctx =>
            {
                if (playerController != null)
                    playerController.SetHoldInput(0f);
            };
        }

        private void Start()
        {
            CurrentState = GameState.Waiting;
            fishCaught = 0;
            fishMissed = 0;

            if (uiManager != null)
            {
                uiManager.UpdateFishCounter(fishCaught, fishMissed);
                uiManager.HideResultPanel();
            }

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.ResetRound();
            }
        }

        private void OnEnable() => inputActions.Enable();

        private void OnDisable()
        {
            inputActions.Disable();

            if (restartLoopRoutine != null)
            {
                StopCoroutine(restartLoopRoutine);
                restartLoopRoutine = null;
            }
        }

        private void Update()
        {
            if (CurrentState == GameState.Waiting && MinigameContext.IsMeasurementActive)
            {
                StartLoop();
            }

            if (CurrentState == GameState.Playing && !MinigameContext.IsMeasurementActive)
            {
                StopLoopSilently();
            }
        }

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            if (!IsGameplayActive || playerController == null) return;

            if (message.StartsWith("Hold:") || message.StartsWith("Time:"))
            {
                string prefix = message.StartsWith("Hold:") ? "Hold:" : "Time:";
                string[] parts = message.Substring(prefix.Length).Split(',');

                if (parts.Length >= 1 && float.TryParse(parts[0], out float holdValue))
                {
                    playerController.SetHoldInput(Mathf.Clamp01(holdValue));
                }

                return;
            }
        }

        private void StartLoop()
        {
            CurrentState = GameState.Playing;

            if (uiManager != null)
            {
                uiManager.HideResultPanel();
                uiManager.UpdateFishCounter(fishCaught, fishMissed);
            }

            if (playerController != null)
            {
                playerController.ResetRound();
                playerController.SetGameplayEnabled(true);
            }
        }

        private void StopLoopSilently()
        {
            CurrentState = GameState.Waiting;

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.SetHoldInput(0f);
            }
        }

        public void OnCatchSuccess()
        {
            if (CurrentState != GameState.Playing) return;

            fishCaught++;

            if (uiManager != null)
            {
                uiManager.UpdateFishCounter(fishCaught, fishMissed);
                uiManager.ShowResultPanel("Fish Caught!", fishCaught, fishMissed);
            }

            EndRoundAndRestart();
        }

        public void OnCatchFailed()
        {
            if (CurrentState != GameState.Playing) return;

            fishMissed++;

            if (uiManager != null)
            {
                uiManager.UpdateFishCounter(fishCaught, fishMissed);
                uiManager.ShowResultPanel("Fish Escaped!", fishCaught, fishMissed);
            }

            EndRoundAndRestart();
        }

        private void EndRoundAndRestart()
        {
            CurrentState = GameState.Results;

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.SetHoldInput(0f);
            }

            if (restartLoopRoutine != null)
            {
                StopCoroutine(restartLoopRoutine);
            }

            restartLoopRoutine = StartCoroutine(RestartLoopFlow());
        }

        private IEnumerator RestartLoopFlow()
        {
            yield return new WaitForSeconds(resultsScreenDuration);

            if (!MinigameContext.IsMeasurementActive)
            {
                restartLoopRoutine = null;
                yield break;
            }

            bool countdownDone = false;

            TransitionOverlayUI.Instance.ShowCountdown(
                countdownStart,
                countdownStepDuration,
                () => countdownDone = true
            );

            yield return new WaitUntil(() => countdownDone);

            if (MinigameContext.IsMeasurementActive)
            {
                CurrentState = GameState.Waiting;
            }

            restartLoopRoutine = null;
        }
    }
}