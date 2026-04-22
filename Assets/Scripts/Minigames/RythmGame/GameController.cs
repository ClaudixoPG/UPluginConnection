using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RythmGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        private PlayerInputActions inputActions;

        [Header("Buttons")]
        [SerializeField] private List<ButtonController> buttons;

        [Header("Music / Scroll")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private BeatScroller beatScroller;
        [SerializeField] private NoteSequenceSpawner noteSequenceSpawner;

        [Header("Loop Flow")]
        [SerializeField] private float fallbackSongDurationSeconds = 36f;
        [SerializeField] private float resultsScreenDuration = 2f;
        [SerializeField] private int countdownStart = 3;
        [SerializeField] private float countdownStepDuration = 1f;

        [Header("UI - Gameplay")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI multiplierText;

        [Header("UI - Results")]
        [SerializeField] private GameObject resultsScreen;
        [SerializeField] private TextMeshProUGUI percentageHitText;
        [SerializeField] private TextMeshProUGUI perfectHitText;
        [SerializeField] private TextMeshProUGUI goodHitText;
        [SerializeField] private TextMeshProUGUI hitText;
        [SerializeField] private TextMeshProUGUI missedText;
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        [Header("Multiplier")]
        [SerializeField] private int[] multiplierThresholds;

        public static GameController instance;

        public enum GameState
        {
            Waiting,
            Playing,
            Results
        }

        public GameState CurrentState { get; private set; } = GameState.Waiting;

        public int currentScore;
        public int currentMultiplier = 1;
        public int multiplierTracker;

        public float totalNotes;
        public float perfectNote;
        public float goodNote;
        public float hitNote;
        public float missedNote;

        private ButtonController activeButton;
        private bool loopEnding;
        private Coroutine restartLoopRoutine;

        private bool IsGameplayActive =>
            CurrentState == GameState.Playing &&
            MinigameContext.IsMeasurementActive;

        private void Awake()
        {
            instance = this;

            inputActions = new PlayerInputActions();
            inputActions.RythmGame.Enable();

            inputActions.RythmGame.X_Button.performed += ctx =>
            {
                if (IsGameplayActive && buttons.Count > 0) buttons[0].PressButton();
            };
            inputActions.RythmGame.X_Button.canceled += ctx =>
            {
                if (buttons.Count > 0) buttons[0].ReleaseButton();
            };

            inputActions.RythmGame.B_Button.performed += ctx =>
            {
                if (IsGameplayActive && buttons.Count > 1) buttons[1].PressButton();
            };
            inputActions.RythmGame.B_Button.canceled += ctx =>
            {
                if (buttons.Count > 1) buttons[1].ReleaseButton();
            };

            inputActions.RythmGame.Y_Button.performed += ctx =>
            {
                if (IsGameplayActive && buttons.Count > 2) buttons[2].PressButton();
            };
            inputActions.RythmGame.Y_Button.canceled += ctx =>
            {
                if (buttons.Count > 2) buttons[2].ReleaseButton();
            };

            inputActions.RythmGame.A_Button.performed += ctx =>
            {
                if (IsGameplayActive && buttons.Count > 3) buttons[3].PressButton();
            };
            inputActions.RythmGame.A_Button.canceled += ctx =>
            {
                if (buttons.Count > 3) buttons[3].ReleaseButton();
            };
        }

        private void Start()
        {
            ResetLoopStats();

            if (resultsScreen != null)
                resultsScreen.SetActive(false);

            if (beatScroller != null)
                beatScroller.ResetScroller();

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.time = 0f;
            }

            CurrentState = GameState.Waiting;
        }

        private void OnEnable() => inputActions.Enable();

        private void OnDisable()
        {
            inputActions.Disable();
            StopCurrentLoopSilently();
        }

        public void HandleMessage(string message)
        {
            if (!IsGameplayActive) return;
            if (string.IsNullOrEmpty(message)) return;

            if (message.StartsWith("Dpad:"))
            {
                ReleaseAllButtons();
                string dir = message.Substring("Dpad:".Length).ToUpper();
                switch (dir)
                {
                    case "UP":
                        if (buttons.Count > 0) buttons[0].PressButton();
                        break;
                    case "DOWN":
                        if (buttons.Count > 1) buttons[1].PressButton();
                        break;
                    case "LEFT":
                        if (buttons.Count > 2) buttons[2].PressButton();
                        break;
                    case "RIGHT":
                        if (buttons.Count > 3) buttons[3].PressButton();
                        break;
                }
                return;
            }

            if (message.StartsWith("DpadRelease:"))
            {
                string dir = message.Substring("DpadRelease:".Length).ToUpper();
                switch (dir)
                {
                    case "UP":
                        if (buttons.Count > 0) buttons[0].ReleaseButton();
                        break;
                    case "DOWN":
                        if (buttons.Count > 1) buttons[1].ReleaseButton();
                        break;
                    case "LEFT":
                        if (buttons.Count > 2) buttons[2].ReleaseButton();
                        break;
                    case "RIGHT":
                        if (buttons.Count > 3) buttons[3].ReleaseButton();
                        break;
                }
            }
        }

        private void Update()
        {
#if UNITY_ANDROID
            HandleTouchInput();
#endif

            if (CurrentState == GameState.Waiting && MinigameContext.IsMeasurementActive)
            {
                StartLoop();
            }

            if (CurrentState == GameState.Playing)
            {
                if (!MinigameContext.IsMeasurementActive)
                {
                    StopCurrentLoopSilently();
                    return;
                }

                if (audioSource != null && !audioSource.isPlaying && !loopEnding)
                {
                    EndLoop();
                }
            }
        }

        private void HandleTouchInput()
        {
            if (!IsGameplayActive) return;
            if (Touchscreen.current == null) return;

            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                int buttonLayerMask = LayerMask.GetMask("Button");
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
                Collider2D hit = Physics2D.OverlapPoint(worldPos, buttonLayerMask);

                if (hit != null && hit.TryGetComponent<ButtonController>(out var button))
                {
                    activeButton = button;
                    activeButton.PressButton();
                }
            }

            if (touch.press.wasReleasedThisFrame && activeButton != null)
            {
                activeButton.ReleaseButton();
                activeButton = null;
            }
        }

        private void StartLoop()
        {
            loopEnding = false;
            CurrentState = GameState.Playing;

            ResetLoopStats();

            if (resultsScreen != null)
                resultsScreen.SetActive(false);

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] != null)
                    buttons[i].ResetButtonState();
            }

            if (beatScroller != null)
                beatScroller.ResetScroller();

            if (noteSequenceSpawner != null && beatScroller != null)
            {
                float songDuration = GetSongDurationSeconds();
                totalNotes = noteSequenceSpawner.GenerateSequence(
                songDuration,
                beatScroller.ScrollSpeedUnitsPerSecond,
                beatScroller);
            }
            else
            {
                totalNotes = 0f;
            }

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.time = 0f;
                audioSource.Play();
            }

            if (beatScroller != null)
                beatScroller.SetStarted(true);
        }

        private void EndLoop()
        {
            loopEnding = true;
            CurrentState = GameState.Results;

            if (beatScroller != null)
                beatScroller.SetStarted(false);

            ShowResults();

            if (restartLoopRoutine != null)
                StopCoroutine(restartLoopRoutine);

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

        private void StopCurrentLoopSilently()
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();

            if (beatScroller != null)
                beatScroller.SetStarted(false);

            if (restartLoopRoutine != null)
            {
                StopCoroutine(restartLoopRoutine);
                restartLoopRoutine = null;
            }

            CurrentState = GameState.Waiting;
            loopEnding = false;
        }

        private void ResetLoopStats()
        {
            currentScore = 0;
            currentMultiplier = 1;
            multiplierTracker = 0;

            perfectNote = 0;
            goodNote = 0;
            hitNote = 0;
            missedNote = 0;

            if (scoreText != null)
                scoreText.text = "Score: 0";

            if (multiplierText != null)
                multiplierText.text = "Multiplier: x1";
        }

        private float GetSongDurationSeconds()
        {
            if (audioSource != null && audioSource.clip != null)
                return audioSource.clip.length;

            return fallbackSongDurationSeconds;
        }

        private void ShowResults()
        {
            if (resultsScreen != null)
                resultsScreen.SetActive(true);

            float totalHit = perfectNote + goodNote + hitNote;
            float percentHit = totalNotes > 0 ? (totalHit / totalNotes) * 100f : 0f;

            if (percentageHitText != null)
                percentageHitText.text = "Hit Percentage: " + percentHit.ToString("F1") + "%";
            if (perfectHitText != null)
                perfectHitText.text = "Perfect Hits: " + perfectNote;
            if (goodHitText != null)
                goodHitText.text = "Good Hits: " + goodNote;
            if (hitText != null)
                hitText.text = "Hits: " + hitNote;
            if (missedText != null)
                missedText.text = "Missed: " + missedNote;
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + currentScore;

            if (rankText != null)
            {
                if (percentHit == 100f) rankText.text = "Rank: S+";
                else if (percentHit >= 95f) rankText.text = "Rank: S";
                else if (percentHit >= 90f) rankText.text = "Rank: A";
                else if (percentHit >= 80f) rankText.text = "Rank: B";
                else if (percentHit >= 70f) rankText.text = "Rank: C";
                else if (percentHit >= 60f) rankText.text = "Rank: D";
                else rankText.text = "Rank: F";
            }
        }

        public void NoteHit(int value)
        {
            multiplierTracker++;

            if (currentMultiplier - 1 < multiplierThresholds.Length)
            {
                if (multiplierTracker >= multiplierThresholds[currentMultiplier - 1])
                {
                    multiplierTracker = 0;
                    currentMultiplier++;
                }
            }

            if (multiplierText != null)
                multiplierText.text = "Multiplier: x" + currentMultiplier;

            currentScore += value * currentMultiplier;

            if (scoreText != null)
                scoreText.text = "Score: " + currentScore;
        }

        public void NoteMissed()
        {
            currentMultiplier = 1;
            multiplierTracker = 0;

            if (multiplierText != null)
                multiplierText.text = "Multiplier: x1";
        }

        // seguridad: liberar todos antes de presionar uno nuevo
        void ReleaseAllButtons()
        {
            foreach (var b in buttons)
                b.ReleaseButton();
        }
    }
}

