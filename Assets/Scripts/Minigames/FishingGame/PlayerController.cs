using UnityEngine;

namespace FishingGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Rail")]
        [SerializeField] private Transform topPivot;
        [SerializeField] private Transform bottomPivot;

        [Header("Fish")]
        [SerializeField] private Transform fish;
        [SerializeField] private float fishMoveIntervalMultiplier = 3f;
        [SerializeField] private float fishSmoothMotion = 1f;

        [Header("Hook")]
        [SerializeField] private Transform hook;
        [SerializeField] private float hookPullPower = 2.5f;
        [SerializeField] private float hookGravityPower = 1.5f;
        [SerializeField] private float hookSizeNormalized = 0.2f;

        [Header("Progress")]
        [SerializeField] private Transform progressBarContainer;
        [SerializeField] private float catchProgressIncreasePerSecond = 0.45f;
        [SerializeField] private float catchProgressDecreasePerSecond = 0.35f;

        [Header("Failure")]
        [SerializeField] private float roundFailDuration = 10f;

        private float fishPositionNormalized;
        private float fishDestinationNormalized;
        private float fishTimer;
        private float fishVelocity;

        private float hookPositionNormalized;
        private float hookPullVelocity;
        private float holdInputNormalized;
        private float catchProgressNormalized;
        private float failTimer;

        private bool gameplayEnabled;

        private void Start()
        {
            ResetRound();
        }

        private void Update()
        {
            if (!gameplayEnabled) return;

            UpdateFish();
            UpdateHook();
            UpdateProgress();
        }

        public void SetGameplayEnabled(bool value)
        {
            gameplayEnabled = value;
        }

        public void SetHoldInput(float value)
        {
            holdInputNormalized = Mathf.Clamp01(value);
        }

        public void ResetRound()
        {
            fishPositionNormalized = 0.5f;
            fishDestinationNormalized = 0.5f;
            fishTimer = 0f;
            fishVelocity = 0f;

            hookPositionNormalized = 0.5f;
            hookPullVelocity = 0f;
            holdInputNormalized = 0f;

            catchProgressNormalized = 0.5f;
            failTimer = roundFailDuration;

            UpdateFishTransform();
            UpdateHookTransform();
            UpdateProgressBar();
        }

        private void UpdateFish()
        {
            fishTimer -= Time.deltaTime;

            if (fishTimer <= 0f)
            {
                fishTimer = Random.value * fishMoveIntervalMultiplier;
                fishDestinationNormalized = Random.value;
            }

            fishPositionNormalized = Mathf.SmoothDamp(
                fishPositionNormalized,
                fishDestinationNormalized,
                ref fishVelocity,
                fishSmoothMotion
            );

            fishPositionNormalized = Mathf.Clamp01(fishPositionNormalized);
            UpdateFishTransform();
        }

        private void UpdateHook()
        {
            hookPullVelocity += holdInputNormalized * hookPullPower * Time.deltaTime;
            hookPullVelocity -= hookGravityPower * Time.deltaTime;

            hookPositionNormalized += hookPullVelocity * Time.deltaTime;

            float maxHookPos = 1f - hookSizeNormalized;

            if (hookPositionNormalized <= 0f && hookPullVelocity < 0f)
            {
                hookPositionNormalized = 0f;
                hookPullVelocity = 0f;
            }

            if (hookPositionNormalized >= maxHookPos && hookPullVelocity > 0f)
            {
                hookPositionNormalized = maxHookPos;
                hookPullVelocity = 0f;
            }

            hookPositionNormalized = Mathf.Clamp(hookPositionNormalized, 0f, maxHookPos);
            UpdateHookTransform();
        }

        private void UpdateProgress()
        {
            float hookMin = hookPositionNormalized;
            float hookMax = hookPositionNormalized + hookSizeNormalized;

            bool fishInside = fishPositionNormalized >= hookMin && fishPositionNormalized <= hookMax;

            if (fishInside)
            {
                catchProgressNormalized += catchProgressIncreasePerSecond * Time.deltaTime;
            }
            else
            {
                catchProgressNormalized -= catchProgressDecreasePerSecond * Time.deltaTime;
                failTimer -= Time.deltaTime;

                if (failTimer <= 0f)
                {
                    Lose();
                    return;
                }
            }

            catchProgressNormalized = Mathf.Clamp01(catchProgressNormalized);
            UpdateProgressBar();

            if (catchProgressNormalized >= 1f)
            {
                Win();
            }
        }

        private void UpdateFishTransform()
        {
            if (fish == null) return;
            fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPositionNormalized);
        }

        private void UpdateHookTransform()
        {
            if (hook == null) return;
            hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPositionNormalized);
        }

        private void UpdateProgressBar()
        {
            if (progressBarContainer == null) return;

            Vector3 localScale = progressBarContainer.localScale;
            localScale.y = catchProgressNormalized;
            progressBarContainer.localScale = localScale;
        }

        private void Win()
        {
            gameplayEnabled = false;
            GameController controller = FindFirstObjectByType<GameController>();
            if (controller != null)
            {
                controller.OnCatchSuccess();
            }
        }

        private void Lose()
        {
            gameplayEnabled = false;
            GameController controller = FindFirstObjectByType<GameController>();
            if (controller != null)
            {
                controller.OnCatchFailed();
            }
        }
    }
}