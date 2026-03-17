using TMPro;
using UnityEngine;

namespace GyroMiniGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        [Header("References")]
        public BoardTiltController boardTiltController;
        public BallGoalTracker goalTracker;
        public Rigidbody ballRigidbody;
        public Transform ballSpawnPoint;
        public Transform targetPoint;
        public Transform tiltBoard;

        [Header("Board Spawn")]
        public float targetSpawnMargin = 1.0f;
        public float minTargetDistanceFromPrevious = 2.0f;

        [Header("Game Rules")]
        public float totalTime = 60f;
        public float comboWindowDuration = 5f;
        public float fallPenaltySeconds = 10f;
        public float fallYThreshold = -3f;
        public float fallCooldown = 1.0f;

        [Header("Combo Rewards")]
        public float streak2Bonus = 3f;
        public float streak5Bonus = 7f;
        public float streak9Bonus = 10f;

        [Header("Status Message")]
        public float statusMessageDuration = 1.5f;

        [Header("UI")]
        public TextMeshProUGUI sourceText;
        public TextMeshProUGUI debugText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI streakText;
        public TextMeshProUGUI comboWindowText;
        public TextMeshProUGUI holdProgressText;
        public TextMeshProUGUI statusText;

        private float _remainingTime;
        private int _score;
        private int _streak;
        private float _comboWindowRemaining;
        private bool _isGameOver;

        private Vector3 _lastTargetLocalPos;

        private float _fallCooldownRemaining;
        private float _statusMessageRemaining;
        private string _temporaryStatusMessage = "";

        private void Awake()
        {
            _remainingTime = totalTime;
        }

        private void Start()
        {
            if (goalTracker != null)
            {
                goalTracker.OnGoalCompleted += HandleGoalCompleted;
            }

            SpawnNewTarget(true);
            SetTemporaryStatus("Status: Reach the target");
            UpdateAllUI();
        }

        private void Update()
        {
            if (_isGameOver)
            {
                UpdateAllUI();
                return;
            }

            _remainingTime -= Time.deltaTime;
            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                _isGameOver = true;
                SetTemporaryStatus("Status: Game Over");
                UpdateAllUI();
                return;
            }

            if (_streak > 0)
            {
                _comboWindowRemaining -= Time.deltaTime;
                if (_comboWindowRemaining <= 0f)
                {
                    _comboWindowRemaining = 0f;
                    _streak = 0;
                    SetTemporaryStatus("Status: Combo lost");
                }
            }

            if (_fallCooldownRemaining > 0f)
            {
                _fallCooldownRemaining -= Time.deltaTime;
            }

            if (_statusMessageRemaining > 0f)
            {
                _statusMessageRemaining -= Time.deltaTime;
                if (_statusMessageRemaining <= 0f)
                {
                    _temporaryStatusMessage = "";
                }
            }

            if (goalTracker != null)
            {
                goalTracker.ManualUpdate();
            }

            CheckBallFall();
            UpdateAllUI();
        }
        private void FixedUpdate()
        {
            if (_isGameOver)
                return;

            if (goalTracker != null)
            {
                goalTracker.ManualFixedUpdate();
            }
        }
        public void HandleMessage(string message)
        {
            if (boardTiltController == null)
                return;

            if (boardTiltController.inputSourceMode != InputSourceMode.RemoteWear)
                return;

            if (GyroMessageParser.TryParseRaw(message, out Vector3 gyro))
            {
                boardTiltController.SetRemoteGyro(gyro);
            }
        }

        private void HandleGoalCompleted()
        {
            _score += 1;

            bool comboStillAlive = _comboWindowRemaining > 0f;
            _streak = comboStillAlive ? _streak + 1 : 1;
            _comboWindowRemaining = comboWindowDuration;

            ApplyStreakReward(_streak);
            SpawnNewTarget(false);

            if (goalTracker != null)
                goalTracker.ResetTracker();

            SetTemporaryStatus("Status: Point scored!");
        }

        private void ApplyStreakReward(int streak)
        {
            if (streak == 2)
            {
                _remainingTime += streak2Bonus;
                SetTemporaryStatus("Status: Combo x2! +3s");
            }
            else if (streak == 5)
            {
                _remainingTime += streak5Bonus;
                SetTemporaryStatus("Status: Combo x5! +7s");
            }
            else if (streak == 9)
            {
                _remainingTime += streak9Bonus;
                SetTemporaryStatus("Status: Combo x9! +10s");
            }
        }

        private void SpawnNewTarget(bool firstSpawn)
        {
            if (targetPoint == null || tiltBoard == null)
                return;

            // Como TargetPoint es hijo de TiltBoard y usamos localPosition,
            // trabajamos en espacio local del tablero.
            // Un Cube base en Unity va de -0.5 a 0.5 en X y Z.

            float boardHalfLocalX = 0.5f;
            float boardHalfLocalZ = 0.5f;

            float targetHalfLocalX = targetPoint.localScale.x * 0.5f;
            float targetHalfLocalZ = targetPoint.localScale.z * 0.5f;

            float minX = -boardHalfLocalX + targetHalfLocalX;
            float maxX = boardHalfLocalX - targetHalfLocalX;

            float minZ = -boardHalfLocalZ + targetHalfLocalZ;
            float maxZ = boardHalfLocalZ - targetHalfLocalZ;

            Vector3 localPos;
            int attempts = 0;

            do
            {
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);
                float y = GetTargetLocalHeight();

                localPos = new Vector3(x, y, z);
                attempts++;
            }
            while (!firstSpawn &&
                   Vector2.Distance(
                       new Vector2(localPos.x, localPos.z),
                       new Vector2(_lastTargetLocalPos.x, _lastTargetLocalPos.z)
                   ) < minTargetDistanceFromPrevious &&
                   attempts < 20);

            targetPoint.localPosition = localPos;
            _lastTargetLocalPos = localPos;
        }
        private float GetTargetLocalHeight()
        {
            float boardTopLocalY = 0.5f;
            float targetHalfLocalY = targetPoint.localScale.y * 0.5f;
            float visualOffset = 0.01f;

            return boardTopLocalY + targetHalfLocalY + visualOffset;
        }

        private void CheckBallFall()
        {
            if (ballRigidbody == null || _fallCooldownRemaining > 0f)
                return;

            if (ballRigidbody.transform.position.y > fallYThreshold)
                return;

            _remainingTime = Mathf.Max(0f, _remainingTime - fallPenaltySeconds);
            _streak = 0;
            _comboWindowRemaining = 0f;
            _fallCooldownRemaining = fallCooldown;

            RespawnBall();
            SetTemporaryStatus($"Status: Fell off! -{fallPenaltySeconds:0}s");
        }

        private void RespawnBall()
        {
            if (ballRigidbody == null || ballSpawnPoint == null)
                return;

            ballRigidbody.linearVelocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            ballRigidbody.position = ballSpawnPoint.position;
            ballRigidbody.rotation = Quaternion.identity;
            ballRigidbody.Sleep();
            ballRigidbody.WakeUp();

            if (boardTiltController != null)
            {
                boardTiltController.ResetBoardTilt();
            }

            if (goalTracker != null)
            {
                goalTracker.ResetTracker();
            }
        }

        private void SetTemporaryStatus(string message)
        {
            _temporaryStatusMessage = message;
            _statusMessageRemaining = statusMessageDuration;
        }

        private void UpdateAllUI()
        {
            if (sourceText != null && boardTiltController != null)
            {
                sourceText.text = $"Source: {boardTiltController.inputSourceMode}";
            }

            if (debugText != null && boardTiltController != null)
            {
                Vector3 input = boardTiltController.CurrentInputVector;
                Vector2 tilt = boardTiltController.CurrentTilt;
                Vector3 tp = targetPoint.localPosition;

                debugText.text =
                    $"gyroX: {input.x:F3}\n" +
                    $"gyroY: {input.y:F3}\n" +
                    $"gyroZ: {input.z:F3}\n" +
                    $"tiltX: {tilt.x:F2}\n" +
                    $"tiltZ: {tilt.y:F2}\n" +
                    $"targetLocal: {tp.x:F2}, {tp.y:F2}, {tp.z:F2}";
            }

            if (timerText != null)
            {
                timerText.text = $"Time: {_remainingTime:F1}s";
            }

            if (scoreText != null)
            {
                scoreText.text = $"Score: {_score}";
            }

            if (streakText != null)
            {
                streakText.text = $"Streak: {_streak}";
            }

            if (comboWindowText != null)
            {
                comboWindowText.text = $"Combo: {_comboWindowRemaining:F1}s";
            }

            if (holdProgressText != null && goalTracker != null)
            {
                float progress = goalTracker.GetHoldProgress01();
                holdProgressText.text = $"Hold: {Mathf.RoundToInt(progress * 100f)}%";
            }

            if (statusText != null)
            {
                if (_isGameOver)
                {
                    statusText.text = "Status: Game Over";
                }
                else if (!string.IsNullOrEmpty(_temporaryStatusMessage))
                {
                    statusText.text = _temporaryStatusMessage;
                }
                else if (goalTracker != null && goalTracker.IsInsideGoal)
                {
                    statusText.text = "Status: Holding target";
                }
                else
                {
                    statusText.text = "Status: Reach the target";
                }
            }
        }
    }
}