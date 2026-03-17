using UnityEngine;

namespace GyroMiniGame
{
    public class BallGoalTracker : MonoBehaviour
    {
        [Header("References")]
        public Transform ball;
        public Rigidbody ballRigidbody;
        public Transform targetPoint;
        public Renderer targetRenderer;

        [Header("Goal Settings")]
        public float goalRadius = 0.75f;
        public float requiredHoldTime = 1.5f;

        [Header("Visual Feedback")]
        public Material outsideMaterial;
        public Material insideMaterial;

        [Header("Assist While Inside Goal")]
        public bool enableAssist = true;
        [Range(0.5f, 1f)] public float insideLinearDamping = 0.88f;
        [Range(0.5f, 1f)] public float insideAngularDamping = 0.85f;

        public float CurrentHoldTime { get; private set; }
        public bool IsInsideGoal { get; private set; }

        public System.Action OnGoalCompleted;

        private bool _goalTriggeredThisStay;
        private bool _previousInsideGoal;

        public void ManualUpdate()
        {
            if (ball == null || targetPoint == null)
                return;

            Vector2 ballPos = new Vector2(ball.position.x, ball.position.z);
            Vector2 targetPos = new Vector2(targetPoint.position.x, targetPoint.position.z);

            float distance = Vector2.Distance(ballPos, targetPos);
            IsInsideGoal = distance <= goalRadius;

            UpdateTargetMaterialIfNeeded();

            if (IsInsideGoal)
            {
                if (!_goalTriggeredThisStay)
                {
                    CurrentHoldTime += Time.deltaTime;

                    if (CurrentHoldTime >= requiredHoldTime)
                    {
                        _goalTriggeredThisStay = true;
                        CurrentHoldTime = requiredHoldTime;
                        OnGoalCompleted?.Invoke();
                    }
                }
            }
            else
            {
                ResetStayStateOnly();
            }
        }

        public void ManualFixedUpdate()
        {
            if (!enableAssist || !IsInsideGoal || ballRigidbody == null)
                return;

            // Frenado suave para ayudar a que la esfera se estabilice sobre el objetivo
#if UNITY_6000_0_OR_NEWER
            ballRigidbody.linearVelocity = new Vector3(
                ballRigidbody.linearVelocity.x * insideLinearDamping,
                ballRigidbody.linearVelocity.y,
                ballRigidbody.linearVelocity.z * insideLinearDamping
            );
#else
            ballRigidbody.velocity = new Vector3(
                ballRigidbody.velocity.x * insideLinearDamping,
                ballRigidbody.velocity.y,
                ballRigidbody.velocity.z * insideLinearDamping
            );
#endif

            ballRigidbody.angularVelocity *= insideAngularDamping;
        }

        public float GetHoldProgress01()
        {
            if (requiredHoldTime <= 0f)
                return 0f;

            return Mathf.Clamp01(CurrentHoldTime / requiredHoldTime);
        }

        public void ResetTracker()
        {
            CurrentHoldTime = 0f;
            IsInsideGoal = false;
            _goalTriggeredThisStay = false;
            _previousInsideGoal = false;
            ApplyOutsideMaterial();
        }

        private void ResetStayStateOnly()
        {
            CurrentHoldTime = 0f;
            _goalTriggeredThisStay = false;
        }

        private void UpdateTargetMaterialIfNeeded()
        {
            if (_previousInsideGoal == IsInsideGoal)
                return;

            _previousInsideGoal = IsInsideGoal;

            if (IsInsideGoal)
                ApplyInsideMaterial();
            else
                ApplyOutsideMaterial();
        }

        private void ApplyInsideMaterial()
        {
            if (targetRenderer != null && insideMaterial != null)
            {
                targetRenderer.material = insideMaterial;
            }
        }

        private void ApplyOutsideMaterial()
        {
            if (targetRenderer != null && outsideMaterial != null)
            {
                targetRenderer.material = outsideMaterial;
            }
        }
    }
}