using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GyroMiniGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        [Header("References")]
        public PlayerController playerController;
        public Transform targetZone;
        public TextMeshProUGUI sourceText;
        public TextMeshProUGUI debugText;
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI progressText;

        [Header("Input Source")]
        public InputSourceMode inputSourceMode = InputSourceMode.LocalPhone;

        [Header("Input Tuning")]
        public float localSensitivity = 2f;
        public float remoteSensitivity = 2f;
        public bool invertAxis = false;
        public float deadZone = 0.05f;

        [Header("MiniGame")]
        public float targetHalfWidth = 1.5f;
        public float requiredTimeInTarget = 5f;

        private Vector3 _remoteGyro;
        private bool _hasRemoteData = false;

        private float _timeInsideTarget = 0f;
        private bool _completed = false;

        private void Start()
        {
            if (inputSourceMode == InputSourceMode.LocalPhone && UnityEngine.InputSystem.Gyroscope.current != null)
            {
                InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
            }

            UpdateSourceLabel();
            UpdateUI();
        }

        private void Update()
        {
            if (_completed)
                return;

            switch (inputSourceMode)
            {
                case InputSourceMode.LocalPhone:
                    UpdateLocalPhoneInput();
                    break;

                case InputSourceMode.RemoteWear:
                    UpdateRemoteWearInput();
                    break;
            }

            UpdateMiniGameState();
            UpdateUI();
        }

        public void HandleMessage(string message)
        {
            if (inputSourceMode != InputSourceMode.RemoteWear)
                return;

            if (GyroMessageParser.TryParseRaw(message, out Vector3 gyro))
            {
                _remoteGyro = gyro;
                _hasRemoteData = true;
            }
        }

        private void UpdateLocalPhoneInput()
        {
            if (UnityEngine.InputSystem.Gyroscope.current == null || !UnityEngine.InputSystem.Gyroscope.current.enabled)
            {
                playerController.SetInput(0f);
                return;
            }

            Vector3 gyro = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();
            float horizontal = gyro.y * localSensitivity;

            if (Mathf.Abs(horizontal) < deadZone)
                horizontal = 0f;

            if (invertAxis)
                horizontal *= -1f;

            playerController.SetInput(horizontal);

            if (debugText != null)
            {
                debugText.text =
                    $"SOURCE: LocalPhone\n" +
                    $"gyroX: {gyro.x:F3}\n" +
                    $"gyroY: {gyro.y:F3}\n" +
                    $"gyroZ: {gyro.z:F3}\n" +
                    $"move: {horizontal:F3}\n" +
                    $"playerX: {playerController.transform.position.x:F3}";
            }
        }

        private void UpdateRemoteWearInput()
        {
            if (!_hasRemoteData)
            {
                playerController.SetInput(0f);

                if (debugText != null)
                {
                    debugText.text =
                        "SOURCE: RemoteWear\n" +
                        "Waiting remote data...";
                }
                return;
            }

            float horizontal = _remoteGyro.y * remoteSensitivity;

            if (Mathf.Abs(horizontal) < deadZone)
                horizontal = 0f;

            if (invertAxis)
                horizontal *= -1f;

            playerController.SetInput(horizontal);

            if (debugText != null)
            {
                debugText.text =
                    $"SOURCE: RemoteWear\n" +
                    $"gyroX: {_remoteGyro.x:F3}\n" +
                    $"gyroY: {_remoteGyro.y:F3}\n" +
                    $"gyroZ: {_remoteGyro.z:F3}\n" +
                    $"move: {horizontal:F3}\n" +
                    $"playerX: {playerController.transform.position.x:F3}";
            }
        }

        private void UpdateMiniGameState()
        {
            float zoneCenterX = targetZone.position.x;
            float playerX = playerController.transform.position.x;

            bool insideTarget = Mathf.Abs(playerX - zoneCenterX) <= targetHalfWidth;

            if (insideTarget)
            {
                _timeInsideTarget += Time.deltaTime;

                if (_timeInsideTarget >= requiredTimeInTarget)
                {
                    _timeInsideTarget = requiredTimeInTarget;
                    _completed = true;
                }
            }
            else
            {
                _timeInsideTarget = Mathf.Max(0f, _timeInsideTarget - Time.deltaTime * 0.5f);
            }

            if (statusText != null)
            {
                statusText.text = _completed
                    ? "Status: Completed"
                    : insideTarget
                        ? "Status: Inside target"
                        : "Status: Outside target";
            }
        }

        private void UpdateUI()
        {
            if (progressText != null)
            {
                float progress = Mathf.Clamp01(_timeInsideTarget / requiredTimeInTarget);
                progressText.text = $"Progress: {Mathf.RoundToInt(progress * 100f)}%";
            }
        }

        private void UpdateSourceLabel()
        {
            if (sourceText != null)
            {
                sourceText.text = $"Input Source: {inputSourceMode}";
            }
        }

        private void OnDisable()
        {
            if (UnityEngine.InputSystem.Gyroscope.current != null && UnityEngine.InputSystem.Gyroscope.current.enabled)
            {
                InputSystem.DisableDevice(UnityEngine.InputSystem.Gyroscope.current);
            }
        }
    }
}