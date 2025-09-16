using SaveSystem;
using TMPro;
using UnityEngine;

namespace MinigameSystem.Minigames.FindYourCredentials
{
    public class FindYourCredential_Minigame : MinigameHandler, IGameController
    {
        private const float MAX_POINTS = 100f;
        private PlayerInputActions inputActions;

        [Header("Settings")]
        [SerializeField] private string[] _randomNames;

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _points_text;
        [SerializeField] private Animator _animator;
        [SerializeField] private InfiniteCarousel _carousel;

        [SerializeField] private CardInfo[] _cards;

        private int _playerIndex;
        private int _points = 100;

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
                _carousel.MoveRight();
            }

            if (direction.x != 0 && direction.x < 0.5f)
            {
                _carousel.MoveLeft();
            }
        }

        public void Fire()
        {
            _carousel.enabled = false;
            inputActions.FindYourCredentialGame.Disable();
            _animator.SetTrigger("Scan");
        }

        public void EndScan()
        {
            if (_playerIndex == _carousel.CurrentIndex)
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
            _carousel.enabled = true;

            _points = Mathf.Clamp(_points - 10, 0, 100);
            _points_text.text = _points.ToString();
        }

        public void UI_WinGame()
        {
            CompleteGame("Find Your Credentials", $"Total Points: {_points}", _points/ MAX_POINTS);
        }

        protected override void OnStartGame()
        {
            foreach (var card in _cards)
            {
                card.SetCardInfo(_randomNames[Random.Range(0, _randomNames.Length)], Random.Range(18, 42).ToString());
            }

            _playerIndex = Random.Range(0, _cards.Length);

            var gameData = SaveHandler.GetGameData();
            _cards[_playerIndex].SetCardInfo(gameData.username, gameData.age.ToString());

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
