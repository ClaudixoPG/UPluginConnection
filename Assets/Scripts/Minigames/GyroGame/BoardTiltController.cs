using UnityEngine;
using UnityEngine.InputSystem;

namespace GyroMiniGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoardTiltController : MonoBehaviour
    {
        [Header("Input Source")]
        public InputSourceMode inputSourceMode = InputSourceMode.LocalPhone;

        [Header("Axis Enable")]
        public bool enableTiltX = true;
        public bool enableTiltZ = true;

        [Header("Tilt Settings")]
        public float localSensitivity = 120f;
        public float remoteSensitivity = 120f;
        public float maxTiltAngle = 12f;
        public float tiltSmoothSpeed = 8f;

        [Header("Noise Filtering")]
        public float deadZoneX = 0.08f;
        public float deadZoneZ = 0.08f;

        [Header("Axis Invert")]
        public bool invertX = false;
        public bool invertZ = false;

        [Header("Optional Reset")]
        public bool autoReturnToCenter = false;
        public float autoReturnSpeed = 2f;

        private Rigidbody _rb;

        private Vector3 _remoteGyro;
        private bool _hasRemoteData;

        private float _targetTiltX;
        private float _targetTiltZ;

        private float _currentTiltX;
        private float _currentTiltZ;

        public Vector3 CurrentInputVector { get; private set; }
        public Vector2 CurrentTilt => new Vector2(_currentTiltX, _currentTiltZ);

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (inputSourceMode == InputSourceMode.LocalPhone && UnityEngine.InputSystem.Gyroscope.current != null)
            {
                InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
            }
        }

        private void Update()
        {
            switch (inputSourceMode)
            {
                case InputSourceMode.LocalPhone:
                    UpdateLocalInput();
                    break;

                case InputSourceMode.RemoteWear:
                    UpdateRemoteInput();
                    break;
            }

            if (autoReturnToCenter)
            {
                if (enableTiltX)
                    _targetTiltX = Mathf.MoveTowards(_targetTiltX, 0f, autoReturnSpeed * Time.deltaTime);

                if (enableTiltZ)
                    _targetTiltZ = Mathf.MoveTowards(_targetTiltZ, 0f, autoReturnSpeed * Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            _currentTiltX = Mathf.Lerp(_currentTiltX, _targetTiltX, Time.fixedDeltaTime * tiltSmoothSpeed);
            _currentTiltZ = Mathf.Lerp(_currentTiltZ, _targetTiltZ, Time.fixedDeltaTime * tiltSmoothSpeed);

            Quaternion targetRotation = Quaternion.Euler(_currentTiltX, 0f, _currentTiltZ);
            _rb.MoveRotation(targetRotation);
        }

        public void SetRemoteGyro(Vector3 gyro)
        {
            _remoteGyro = gyro;
            _hasRemoteData = true;
        }

        public void ResetBoardTilt()
        {
            _targetTiltX = 0f;
            _targetTiltZ = 0f;
            _currentTiltX = 0f;
            _currentTiltZ = 0f;
            _rb.MoveRotation(Quaternion.identity);
        }

        private void UpdateLocalInput()
        {
            if (UnityEngine.InputSystem.Gyroscope.current == null || !UnityEngine.InputSystem.Gyroscope.current.enabled)
            {
                CurrentInputVector = Vector3.zero;
                return;
            }

            Vector3 gyro = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();
            CurrentInputVector = gyro;

            ApplyIncrementalTilt(gyro, localSensitivity);
        }

        private void UpdateRemoteInput()
        {
            if (!_hasRemoteData)
            {
                CurrentInputVector = Vector3.zero;
                return;
            }

            CurrentInputVector = _remoteGyro;
            ApplyIncrementalTilt(_remoteGyro, remoteSensitivity);
        }

        private void ApplyIncrementalTilt(Vector3 gyro, float sensitivity)
        {
            float inputX = gyro.x;
            float inputZ = gyro.y;

            if (Mathf.Abs(inputX) < deadZoneX)
                inputX = 0f;

            if (Mathf.Abs(inputZ) < deadZoneZ)
                inputZ = 0f;

            if (invertX) inputX *= -1f;
            if (invertZ) inputZ *= -1f;

            if (enableTiltX)
            {
                _targetTiltX += inputX * sensitivity * Time.deltaTime;
                _targetTiltX = Mathf.Clamp(_targetTiltX, -maxTiltAngle, maxTiltAngle);
            }

            if (enableTiltZ)
            {
                _targetTiltZ += inputZ * sensitivity * Time.deltaTime;
                _targetTiltZ = Mathf.Clamp(_targetTiltZ, -maxTiltAngle, maxTiltAngle);
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