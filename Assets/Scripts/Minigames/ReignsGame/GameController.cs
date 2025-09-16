using MinigameSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ReignsGame
{
    public class GameController : MinigameHandler, IGameController
    {
        [SerializeField] private TextMeshProUGUI _points_text;
        [SerializeField] private Transform _pivot;
        [SerializeField] private Animator _topAnimator;
        [SerializeField] private Animator _bottomAnimator;

        private int _currentIndex;
        private int _points = 100;

        private PlayerInputActions inputActions;
        public PlayerController playerController;

        private void OnEnable() => inputActions?.Enable();
        private void OnDisable() => inputActions?.Disable();

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // --- Movimiento continuo ---
            if (message.StartsWith("Joystick:"))
            {
                string[] parts = message.Substring("Joystick:".Length).Split(',');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y))
                {
                    playerController.moveInput = new Vector2(x, y);
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
                    Vector2 releaseInput = new Vector2(x, y);
                    playerController.OnJoystickRelease(releaseInput);
                }
                return;
            }

            Debug.LogWarning("Formato de mensaje no reconocido: " + message);
        }

        protected override void OnStartGame()
        {
            inputActions = new PlayerInputActions();
            inputActions.ReignsGame.Enable();

            // --- InputActions (para test en PC / Unity Input System) ---
            inputActions.ReignsGame.Move.performed += ctx => playerController.moveInput = ctx.ReadValue<Vector2>();

            //inputActions.ReignsGame.Move.canceled += ctx => playerController.moveInput = Vector2.zero;
            inputActions.ReignsGame.Move.canceled += ctx => playerController.OnJoystickRelease(playerController.moveInput);

            foreach (Transform child in _pivot)
            {
                child.gameObject.SetActive(false);
            }

            ShuffleChildren(_pivot);
            _pivot.GetChild(_currentIndex).gameObject.SetActive(true);
        }

        public void NextTarget()
        {
            _pivot.GetChild(_currentIndex).gameObject.SetActive(false);

            _currentIndex++;

            if (_currentIndex >= _pivot.childCount)
            {
                CompleteGame("Reigns", $"Total Points: {_points}");
                return;
            }

            _pivot.GetChild(_currentIndex).gameObject.SetActive(true);
        }

        public void CheckTarget(int side)
        {
            if (!_pivot.GetChild(_currentIndex).GetComponent<CondomBox>().isCorrect)
            {
                if (side == 1)
                {
                    _topAnimator.SetTrigger("Success");
                }else if( side == -1)
                {
                    _bottomAnimator.SetTrigger("Success");
                }
            }
            else
            {
                _points = Mathf.Clamp(_points - 10, 0, 100);

                if (side == 1)
                {
                    _topAnimator.SetTrigger("Fail");
                }
                else if (side == -1)
                {
                    _bottomAnimator.SetTrigger("Fail");
                }
            }

            _points_text.text = _points.ToString();
            playerController.ResetController();
            NextTarget();
        }

        private static void ShuffleChildren(Transform parent)
        {
            if (parent == null || parent.childCount <= 1)
                return;

            // Store children in a list
            List<Transform> children = new List<Transform>();
            foreach (Transform child in parent)
            {
                children.Add(child);
            }

            // Shuffle the list (Fisher–Yates algorithm)
            for (int i = children.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                var temp = children[i];
                children[i] = children[randomIndex];
                children[randomIndex] = temp;
            }

            // Apply the new order back to the hierarchy
            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }

        protected override void UpdateGame()
        {
            
        }
    }
}
