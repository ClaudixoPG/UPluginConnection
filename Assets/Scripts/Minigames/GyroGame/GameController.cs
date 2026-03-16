using TMPro;
using UnityEngine;

namespace GyroMiniGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        [Header("References")]
        public PlayerController playerController;
        public TextMeshProUGUI sourceText;
        public TextMeshProUGUI debugText;

        [Header("Input Source")]
        public InputSourceMode inputSourceMode = InputSourceMode.LocalPhone;

        [Header("Tuning")]
        public float localSensitivity = 5f;
        public float remoteSensitivity = 2f;
        public bool invertAxis = false;
        public float deadZone = 0.05f;

        private Vector3 _remoteGyro;
        private bool _hasRemoteData = false;

        private void Awake()
        {
            Debug.Log("[GyroMiniGame] Awake llamado.");
        }

        private void Start()
        {
            Debug.Log("[GyroMiniGame] Start llamado.");
            Debug.Log("[GyroMiniGame] InputSourceMode = " + inputSourceMode);

            Input.gyro.enabled = true;
            UpdateSourceLabel();

            if (debugText != null)
            {
                debugText.text = "GameController iniciado.\nEsperando datos...";
            }
        }

        private void Update()
        {
            if (debugText == null)
            {
                Debug.LogWarning("[GyroMiniGame] debugText no asignado.");
            }

            if (playerController == null)
            {
                Debug.LogError("[GyroMiniGame] playerController no asignado.");
                return;
            }

            switch (inputSourceMode)
            {
                case InputSourceMode.LocalPhone:
                    UpdateLocalPhoneInput();
                    break;

                case InputSourceMode.RemoteWear:
                    UpdateRemoteWearInput();
                    break;
            }
        }

        public void HandleMessage(string message)
        {
            if (inputSourceMode != InputSourceMode.RemoteWear)
                return;

            if (GyroMessageParser.TryParseRaw(message, out Vector3 gyro))
            {
                _remoteGyro = gyro;
                _hasRemoteData = true;

                Debug.Log($"[GyroMiniGame] Mensaje remoto recibido: {gyro}");
            }
        }
        /*
        private void UpdateLocalPhoneInput()
        {
            Vector3 accel = Input.acceleration;
            float horizontal = accel.x;

            if (Mathf.Abs(horizontal) < deadZone)
                horizontal = 0f;

            horizontal *= localSensitivity;

            if (invertAxis)
                horizontal *= -1f;

            Debug.Log("[GyroMiniGame] Local accel: " + accel + ", horizontal: " + horizontal);

            playerController.SetInput(horizontal);

            if (sourceText != null)
                sourceText.text = "Input Source: LocalPhone";

            if (debugText != null)
            {
                debugText.text =
                    $"SOURCE: LocalPhone\n" +
                    $"accelX: {accel.x:F3}\n" +
                    $"accelY: {accel.y:F3}\n" +
                    $"accelZ: {accel.z:F3}\n" +
                    $"horizontal: {horizontal:F3}\n" +
                    $"playerX: {playerController.transform.position.x:F3}";
            }
        }*/

        private void UpdateLocalPhoneInput()
        {
            Vector3 gyro = Input.gyro.rotationRateUnbiased;

            float horizontal = gyro.y * localSensitivity;

            if (Mathf.Abs(horizontal) < deadZone)
                horizontal = 0f;

            if (invertAxis)
                horizontal *= -1f;

            playerController.SetInput(horizontal);

            if (sourceText != null)
                sourceText.text = "Input Source: LocalPhone";

            if (debugText != null)
            {
                debugText.text =
                    $"SOURCE: LocalPhone\n" +
                    $"gyroX: {gyro.x:F3}\n" +
                    $"gyroY: {gyro.y:F3}\n" +
                    $"gyroZ: {gyro.z:F3}\n" +
                    $"horizontal: {horizontal:F3}\n" +
                    $"playerX: {playerController.transform.position.x:F3}";
            }
        }

        private void UpdateRemoteWearInput()
        {
            if (!_hasRemoteData)
            {
                playerController.SetInput(0f);

                if (sourceText != null)
                    sourceText.text = "Input Source: RemoteWear";

                if (debugText != null)
                {
                    debugText.text =
                        $"SOURCE: RemoteWear\n" +
                        $"Sin datos remotos";
                }
                return;
            }

            float horizontal = _remoteGyro.y;

            if (Mathf.Abs(horizontal) < deadZone)
                horizontal = 0f;

            horizontal *= remoteSensitivity;

            if (invertAxis)
                horizontal *= -1f;

            playerController.SetInput(horizontal);

            if (sourceText != null)
                sourceText.text = "Input Source: RemoteWear";

            if (debugText != null)
            {
                debugText.text =
                    $"SOURCE: RemoteWear\n" +
                    $"gyroX: {_remoteGyro.x:F3}\n" +
                    $"gyroY: {_remoteGyro.y:F3}\n" +
                    $"gyroZ: {_remoteGyro.z:F3}\n" +
                    $"horizontal: {horizontal:F3}\n" +
                    $"playerX: {playerController.transform.position.x:F3}";
            }
        }

        private void UpdateSourceLabel()
        {
            if (sourceText != null)
            {
                sourceText.text = $"Input Source: {inputSourceMode}";
            }
        }
    }
}