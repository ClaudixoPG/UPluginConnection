using NUnit.Framework;
using SpaceShip;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RythmGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        PlayerInputActions inputActions;
        public List<ButtonController> buttons;

        public AudioSource AudioSource;
        public bool startPlaying;
        public BeatScroller beatScroller;

        public static GameController instance;

        //Points
        public int currentScore;

        //multiplier
        public int currentMultiplier;
        public int multiplierTracker; // cada vez que aciertes una nota, se incrementa en 1
        public int[] multiplierThresholds; // los umbrales para incrementar el multiplicador

        //UI
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI multiplierText;

        //Total notes (for accuracy)
        public float totalNotes;
        public float perfectNote;
        public float goodNote;
        public float hitNote;
        public float missedNote;

        //UI (esto debiera ser con un panel manager al cual le paso los values)
        public GameObject resultsScreen;
        public TextMeshProUGUI percentageHitText;
        public TextMeshProUGUI perfectHitText;
        public TextMeshProUGUI goodHitText;
        public TextMeshProUGUI hitText;
        public TextMeshProUGUI missedText;
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI finalScoreText;

        private void Start()
        {
            instance = this;

            //get notes
            totalNotes = FindObjectsByType<Note>(sortMode: FindObjectsSortMode.None).Length;
        }

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.RythmGame.Enable();

            inputActions.RythmGame.X_Button.performed += ctx => buttons[0].PressButton();
            inputActions.RythmGame.X_Button.canceled += ctx => buttons[0].ReleaseButton();
            inputActions.RythmGame.B_Button.performed += ctx => buttons[1].PressButton();
            inputActions.RythmGame.B_Button.canceled += ctx => buttons[1].ReleaseButton();
            inputActions.RythmGame.Y_Button.performed += ctx => buttons[2].PressButton();
            inputActions.RythmGame.Y_Button.canceled += ctx => buttons[2].ReleaseButton();
            inputActions.RythmGame.A_Button.performed += ctx => buttons[3].PressButton();
            inputActions.RythmGame.A_Button.canceled += ctx => buttons[3].ReleaseButton();
        }
        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // --- Dpad:UP / DOWN / LEFT / RIGHT ---
            if (message.StartsWith("Dpad:"))
            {
                string dir = message.Substring("Dpad:".Length).ToUpper();
                switch (dir)
                {
                    case "LEFT":
                        buttons[0].PressButton();
                        break;
                    case "UP":
                        buttons[1].PressButton();
                        break;
                    case "RIGHT":
                        buttons[2].PressButton();
                        break;
                    case "DOWN":
                        buttons[3].PressButton();
                        break;
                    default:
                        Debug.LogWarning("Dirección Dpad desconocida: " + dir);
                        break;
                }
                return;
            }

            // --- DpadRelease:UP / DOWN / LEFT / RIGHT ---
            if (message.StartsWith("DpadRelease:"))
            {
                string dir = message.Substring("DpadRelease:".Length).ToUpper();
                switch (dir)
                {
                    case "LEFT":
                        buttons[0].ReleaseButton();
                        break;
                    case "UP":
                        buttons[1].ReleaseButton();
                        break;
                    case "RIGHT":
                        buttons[2].ReleaseButton();
                        break;
                    case "DOWN":
                        buttons[3].ReleaseButton();
                        break;
                    default:
                        Debug.LogWarning("Dirección Dpad desconocida: " + dir);
                        break;
                }
                return;
            }

            Debug.LogWarning("Formato de mensaje no reconocido: " + message);
        }

        private ButtonController activeButton; // botón actualmente presionado

        void Update()
        {
            #if UNITY_ANDROID
            // --- Touch en móvil ---
            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;

                if (touch.press.wasPressedThisFrame)
                {
                    int buttonLayerMask = LayerMask.GetMask("Button");

                    Vector2 worldPos = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
                    Collider2D hit = Physics2D.OverlapPoint(worldPos,buttonLayerMask);

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
            #endif

            if (startPlaying)
            {
                AudioSource.Play();
                beatScroller.hasStarted = true;
                startPlaying = false;
            }else
            {
                if (!AudioSource.isPlaying && !resultsScreen.activeInHierarchy)
                {
                    //show results screen
                    resultsScreen.SetActive(true);
                    //calculate percentage hit
                    float totalHit = perfectNote + goodNote + hitNote;
                    float percentHit = (totalHit / totalNotes) * 100f;
                    percentageHitText.text = "Hit Percentage: " + percentHit.ToString("F1") + "%";
                    perfectHitText.text = "Perfect Hits: " + perfectNote;
                    goodHitText.text = "Good Hits: " + goodNote;
                    hitText.text = "Hits: " + hitNote;
                    missedText.text = "Missed: " + missedNote;
                    finalScoreText.text = "Final Score: " + currentScore;
                    //rank
                    if (percentHit == 100f) rankText.text = "Rank: S+";
                    else if (percentHit >= 95f) rankText.text = "Rank: S";
                    else if (percentHit >= 90f) rankText.text = "Rank: A";
                    else if (percentHit >= 80f) rankText.text = "Rank: B";
                    else if (percentHit >= 70f) rankText.text = "Rank: C";
                    else if (percentHit >= 60f) rankText.text = "Rank: D";
                    else rankText.text = "Rank: F";
                }
            }
        }

        public void NoteHit(int value)
        {
            multiplierTracker++;
            if (currentMultiplier - 1 < multiplierThresholds.Length) // evitar desbordamiento del array
            {
                if (multiplierTracker >= multiplierThresholds[currentMultiplier - 1]) // si el contador alcanza el umbral
                {
                    multiplierTracker = 0; // reiniciar el contador
                    currentMultiplier++; // incrementar el multiplicador
                }
            }
            multiplierText.text = "Multiplier: x" + currentMultiplier;
            currentScore += value * currentMultiplier;
            scoreText.text = "Score: " + currentScore;
        }

        public void NoteMissed()
        {
            currentMultiplier = 1;
            multiplierTracker = 0;
            multiplierText.text = "Multiplier: x" + currentMultiplier;
        }
    }

}