using MinigameSystem;
using SaveSystem;
using TMPro;
using UnityEngine;

namespace FindYourCredentials
{
    public class GameController : MinigameHandler, IGameController
    {
        private const float MAX_POINTS = 100f;
        private PlayerInputActions inputActions;

        [Header("Settings")]
        [SerializeField] private string[] _randomNames;

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _points_text;
        [SerializeField] private Animator _animator;
        [SerializeField] private CredentialsCarousel _carousel;
        
        private int _points = 100;

        public CredentialData currentInspectingData;

        private void Fire_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Fire();
        }

        private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Move(Vector2.zero);
        }

        private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Move(obj.ReadValue<Vector2>());
        }

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (!inputActions.FindYourCredentialGame.enabled) return;

            // --- Movimiento continuo ---
            if (message.StartsWith("Joystick:"))
            {
                string[] parts = message.Substring("Joystick:".Length).Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    Move(new Vector2(x, y));
                }
                return;
            }

            // --- Release (decisión final, usa eje Y) ---
            if (message.StartsWith("JoystickRelease:"))
            {
                string[] parts = message.Substring("JoystickRelease:".Length).Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    Move(Vector2.zero);
                }
                return;
            }

            // --- Fire ---
            if (message.StartsWith("Tap"))
            {
                Fire();
                return;
            }

            Debug.LogWarning("Formato de mensaje no reconocido: " + message);
        }

        private void Move(Vector2 direction)
        {
            if (direction.x != 0 && direction.x > 0.5f)
            {
                _carousel.Next();
            }

            if (direction.x != 0 && direction.x < 0.5f)
            {
                _carousel.Previous();
            }
        }

        public void Fire()
        {
            inputActions.FindYourCredentialGame.Disable();
            _animator.SetTrigger("Scan");
        }

        public void EndScan()
        {
            if (currentInspectingData != null && currentInspectingData.isCorrect)
            {
                SuccessScan();
            }
            else
            {
                FailScan();
            }
        }

        private void SuccessScan()
        {
            _animator.SetTrigger("Success");
        }

        private void FailScan()
        {
            _animator.SetTrigger("Fail");
            inputActions.FindYourCredentialGame.Enable();

            _points = Mathf.Clamp(_points - 10, 0, 100);
            _points_text.text = _points.ToString();
        }

        public void UI_WinGame()
        {
            CompleteGame("Find Your Credentials", _points/ MAX_POINTS);
        }

        protected override void OnStartGame()
        {
            var credentials = new CredentialData[10];

            for (int i = 0; i < credentials.Length; i++)
            {
                credentials[i] = new CredentialData(Random.Range(0, 10), _randomNames[Random.Range(0, _randomNames.Length)], Random.Range(18, 42).ToString());
            }

            var gameData = SaveHandler.GetGameData();
            var myCredential = new CredentialData(Random.Range(0, 10), gameData.username, gameData.age.ToString());
            myCredential.isCorrect = true;
            credentials[Random.Range(0, credentials.Length)] = myCredential;

            _carousel.SetData(credentials);

            // Inputs
            inputActions = new PlayerInputActions();
            inputActions.FindYourCredentialGame.Enable();

            // --- InputActions (para test en PC / Unity Input System) ---
            inputActions.FindYourCredentialGame.Move.performed += Move_performed;
            inputActions.FindYourCredentialGame.Move.canceled += Move_canceled;
            inputActions.FindYourCredentialGame.Fire.performed += Fire_performed;
        }

        protected override void UpdateGame()
        {
            
        }
    }
}
