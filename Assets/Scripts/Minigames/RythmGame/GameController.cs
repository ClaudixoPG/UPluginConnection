using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        public int scorePerNote = 100;

        //multiplier
        public int currentMultiplier;
        public int multiplierTracker; // cada vez que aciertes una nota, se incrementa en 1
        public int[] multiplierThresholds; // los umbrales para incrementar el multiplicador

        //UI
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI multiplierText;

        private void Start()
        {
            instance = this;
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
            throw new System.NotImplementedException();
        }

        void Update()
        {
            if (startPlaying)
            {
                AudioSource.Play();
                beatScroller.hasStarted = true;
                startPlaying = false;
            }
        }

        public void NoteHit()
        {
            Debug.Log("Hit on time");

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
            currentScore += scorePerNote * currentMultiplier;
            scoreText.text = "Score: " + currentScore;
        }

        public void NoteMissed()
        {
            Debug.Log("Missed the note");

            currentMultiplier = 1;
            multiplierTracker = 0;
            multiplierText.text = "Multiplier: x" + currentMultiplier;

        }


    }
}