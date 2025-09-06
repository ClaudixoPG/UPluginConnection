#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

namespace ReignsGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform targetObject;

        [Header("Parámetros")]
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField, Range(0f, 1f)] private float deadzonePercent = 0.02f;
        [SerializeField, Range(0f, 1f)] private float decisionThresholdPercent = 0.25f;

        [HideInInspector] public Vector2 moveInput = Vector2.zero;

        private float minY = 0f;
        private float maxY = 5f;
        private float deadzone;
        private float decisionThreshold;

        private Vector3 initialPosition;
        private Camera mainCamera;

        private void Reset()
        {
            if (targetObject == null) targetObject = this.transform;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            if (!targetObject) targetObject = this.transform;

            initialPosition = targetObject.position;

            UpdateVerticalLimits();
            UpdateDeadzoneAndThreshold();
        }

        private void Update()
        {
            HandleTurning();
            HandleTranslating();
        }

        #region Translación
        private void HandleTranslating()
        {
            float y = moveInput.y;
            if (Mathf.Abs(y) > deadzone)
            {
                Vector3 newPosition = targetObject.position;
                newPosition.y += y * moveSpeed * Time.deltaTime;
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                targetObject.position = newPosition;
            }
        }
        #endregion

        #region Rotación
        private void HandleTurning()
        {
            float x = moveInput.x;
            if (Mathf.Abs(x) > deadzone)
            {
                if (x > 0f) TurnRight(x);
                else TurnLeft(x);
            }
        }

        public void TurnRight(float inputX)
        {
            float amount = inputX * rotationSpeed * Time.deltaTime;
            targetObject.Rotate(Vector3.up, amount, Space.World);
        }

        public void TurnLeft(float inputX)
        {
            float amount = inputX * rotationSpeed * Time.deltaTime;
            targetObject.Rotate(Vector3.up, amount, Space.World);
        }
        #endregion

        #region Joystick Release
        public void OnJoystickRelease(Vector2 releaseInput)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(targetObject.position);

            float topThreshold = 1f - decisionThresholdPercent;
            float bottomThreshold = decisionThresholdPercent;

            if (viewportPos.y >= topThreshold)
            {
                AddObject();
            }
            else if (viewportPos.y <= bottomThreshold)
            {
                RemoveObject();
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(LerpToInitialPosition());
            }

            moveInput = Vector2.zero;
        }

        private IEnumerator LerpToInitialPosition()
        {
            Vector3 startPos = targetObject.position;
            float t = 0f;
            float duration = 0.3f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                targetObject.position = Vector3.Lerp(startPos, initialPosition, t);
                yield return null;
            }

            targetObject.position = initialPosition;
        }
        #endregion

        #region Métodos dinámicos de límites verticales
        public void UpdateVerticalLimits()
        {
            if (mainCamera == null) mainCamera = Camera.main;

            if (mainCamera.orthographic)
            {
                maxY = mainCamera.orthographicSize;
                minY = -mainCamera.orthographicSize;
            }
            else
            {
                Vector3 bottomWorld = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, Mathf.Abs(mainCamera.transform.position.z)));
                Vector3 topWorld = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, Mathf.Abs(mainCamera.transform.position.z)));
                minY = bottomWorld.y - initialPosition.y;
                maxY = topWorld.y - initialPosition.y;
            }
        }

        public void UpdateDeadzoneAndThreshold()
        {
            if (mainCamera == null) mainCamera = Camera.main;

            deadzone = deadzonePercent;
            decisionThreshold = decisionThresholdPercent;
        }
        #endregion

        #region Add / Remove
        private void AddObject()
        {
            Debug.Log("AddObject -> llamado (no implementado)");
        }

        private void RemoveObject()
        {
            Debug.Log("RemoveObject -> llamado (no implementado)");
        }
        #endregion

        #region Debug Gizmos
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (targetObject == null) targetObject = this.transform;
            if (mainCamera == null) mainCamera = Camera.main;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(initialPosition, 0.1f);

            Gizmos.color = Color.yellow;
            Vector3 minPos = new Vector3(initialPosition.x, minY, initialPosition.z);
            Vector3 maxPos = new Vector3(initialPosition.x, maxY, initialPosition.z);
            Gizmos.DrawLine(minPos, maxPos);

            // Thresholds
            Vector3 bottomThreshold = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, decisionThresholdPercent, Mathf.Abs(mainCamera.transform.position.z)));
            Vector3 topThreshold = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f - decisionThresholdPercent, Mathf.Abs(mainCamera.transform.position.z)));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(bottomThreshold.x - 1, bottomThreshold.y, bottomThreshold.z),
                            new Vector3(bottomThreshold.x + 1, bottomThreshold.y, bottomThreshold.z));

            Gizmos.DrawLine(new Vector3(topThreshold.x - 1, topThreshold.y, topThreshold.z),
                            new Vector3(topThreshold.x + 1, topThreshold.y, topThreshold.z));
        }
#endif
        #endregion
    }
}
