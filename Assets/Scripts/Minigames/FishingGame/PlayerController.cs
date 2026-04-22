using UnityEngine;
using System.Collections;

namespace FishingGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Rail")]
        [SerializeField] private Transform topPivot;
        [SerializeField] private Transform bottomPivot;

        [Header("Fish")]
        [SerializeField] private Transform fish;

        [Header("Hook")]
        [SerializeField] private Transform hook;
        [SerializeField] private float hookSizeNormalized = 0.2f;

        [Header("Progress")]
        [SerializeField] private Transform progressBarContainer;
        [SerializeField] private float initialCatchProgress = 0.5f;

        [Header("Rarity presets")]
        [SerializeField] private FishBehaviourPreset normalPreset;
        [SerializeField] private FishBehaviourPreset rarePreset;
        [SerializeField] private FishBehaviourPreset legendaryPreset;

        private float fishPositionNormalized;
        private float fishDestinationNormalized;
        private float fishTimer;
        private float fishVelocity;

        // Mapeo 1:1: el valor recibido se usa directamente como posición del hook.
        private float hookPositionNormalized;
        private float holdInputNormalized;

        private float catchProgressNormalized;

        private bool gameplayEnabled;
        private bool hasReceivedRemoteInput;
        private FishBehaviourPreset currentPreset;

        private enum FishEncounterState
        {
            Hidden,
            Entering,
            Active,
            Caught,
            Escaped
        }

        private FishEncounterState fishState = FishEncounterState.Hidden;
        private bool IsFishActive => fishState == FishEncounterState.Active;

        [System.Serializable]
        public class FishBehaviourPreset
        {
            [Header("Movement")]
            public float moveIntervalMultiplier = 3f;
            public float smoothMotion = 1f;

            [Header("Catch Balance")]
            public float catchFillPerSecond = 0.45f;
            public float catchDrainPerSecond = 0.35f;
        }

        private Coroutine encounterRoutine;

        private void Start()
        {
            if (normalPreset == null) normalPreset = new FishBehaviourPreset
            {
                moveIntervalMultiplier = 3f,
                smoothMotion = 1f,
                catchFillPerSecond = 0.55f,
                catchDrainPerSecond = 0.22f
            };

            if (rarePreset == null) rarePreset = new FishBehaviourPreset
            {
                moveIntervalMultiplier = 1.8f,
                smoothMotion = 0.7f,
                catchFillPerSecond = 0.38f,
                catchDrainPerSecond = 0.38f
            };

            if (legendaryPreset == null) legendaryPreset = new FishBehaviourPreset
            {
                moveIntervalMultiplier = 0.9f,
                smoothMotion = 0.4f,
                catchFillPerSecond = 0.22f,
                catchDrainPerSecond = 0.55f
            };

            currentPreset = normalPreset;
            ResetRound();
        }

        private void Update()
        {
            if (!gameplayEnabled) return;

            UpdateFishVisualMotion();

            if (!IsFishActive) return;

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
            hasReceivedRemoteInput = true;
        }

        public void ConfigureFish(GameController.FishRarity rarity)
        {
            switch (rarity)
            {
                case GameController.FishRarity.Normal:
                    currentPreset = normalPreset;
                    break;
                case GameController.FishRarity.Rare:
                    currentPreset = rarePreset;
                    break;
                case GameController.FishRarity.Legendary:
                    currentPreset = legendaryPreset;
                    break;
                default:
                    currentPreset = normalPreset;
                    break;
            }
        }

        public void ResetRound()
        {
            fishPositionNormalized = 0.5f;
            fishDestinationNormalized = 0.5f;
            fishTimer = 0f;
            fishVelocity = 0f;

            hookPositionNormalized = 0.5f;
            holdInputNormalized = 0f;
            hasReceivedRemoteInput = false;

            catchProgressNormalized = initialCatchProgress;

            fishState = FishEncounterState.Hidden;

            UpdateFishTransform();
            UpdateHookTransform();
            UpdateProgressBar();
        }

        private void UpdateFishVisualMotion()
        {
            if (fishState != FishEncounterState.Active)
                return;

            fishTimer -= Time.deltaTime;

            if (fishTimer <= 0f)
            {
                fishTimer = Random.value * currentPreset.moveIntervalMultiplier;
                fishDestinationNormalized = Random.value;
            }

            fishPositionNormalized = Mathf.SmoothDamp(
                fishPositionNormalized,
                fishDestinationNormalized,
                ref fishVelocity,
                currentPreset.smoothMotion
            );

            fishPositionNormalized = Mathf.Clamp01(fishPositionNormalized);
            UpdateFishTransform();
        }

        private void UpdateHook()
        {
            if (!hasReceivedRemoteInput)
            {
                UpdateHookTransform();
                return;
            }

            // Mapeo 1:1 directo
            hookPositionNormalized = holdInputNormalized;
            UpdateHookTransform();
        }

        private void UpdateProgress()
        {
            float hookMin = hookPositionNormalized;
            float hookMax = hookPositionNormalized + hookSizeNormalized;

            bool fishInside = fishPositionNormalized >= hookMin && fishPositionNormalized <= hookMax;

            if (fishInside)
            {
                catchProgressNormalized += currentPreset.catchFillPerSecond * Time.deltaTime;
            }
            else
            {
                catchProgressNormalized -= currentPreset.catchDrainPerSecond * Time.deltaTime;
            }

            catchProgressNormalized = Mathf.Clamp01(catchProgressNormalized);
            UpdateProgressBar();

            if (catchProgressNormalized >= 1f)
            {
                Win();
                return;
            }

            if (catchProgressNormalized <= 0f)
            {
                Lose();
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

        public void BeginFishEncounter()
        {
            if (encounterRoutine != null)
                StopCoroutine(encounterRoutine);

            encounterRoutine = StartCoroutine(BeginFishEncounterRoutine());
        }

        private IEnumerator BeginFishEncounterRoutine()
        {
            fishState = FishEncounterState.Entering;
            gameplayEnabled = false;

            float spawnSide = Random.value > 0.5f ? 1.15f : -0.15f;
            fishPositionNormalized = spawnSide;
            fishDestinationNormalized = Random.Range(0.2f, 0.8f);

            UpdateFishTransform();
            UpdateHookTransform();
            UpdateProgressBar();

            float duration = 0.6f;
            float t = 0f;
            float start = fishPositionNormalized;
            float target = fishDestinationNormalized;

            while (t < duration)
            {
                t += Time.deltaTime;
                fishPositionNormalized = Mathf.Lerp(start, target, t / duration);
                UpdateFishTransform();
                yield return null;
            }

            fishPositionNormalized = target;
            fishDestinationNormalized = Random.value;
            fishTimer = 0f;

            gameplayEnabled = true;
            fishState = FishEncounterState.Active;
            encounterRoutine = null;
        }

        public void ResolveFish(bool caught)
        {
            if (encounterRoutine != null)
                StopCoroutine(encounterRoutine);

            encounterRoutine = StartCoroutine(ResolveFishRoutine(caught));
        }

        private IEnumerator ResolveFishRoutine(bool caught)
        {
            gameplayEnabled = false;
            fishState = caught ? FishEncounterState.Caught : FishEncounterState.Escaped;

            float duration = 0.5f;
            float t = 0f;
            float start = fishPositionNormalized;
            float target = caught ? 1.2f : -0.2f;

            while (t < duration)
            {
                t += Time.deltaTime;
                fishPositionNormalized = Mathf.Lerp(start, target, t / duration);
                UpdateFishTransform();
                yield return null;
            }

            fishState = FishEncounterState.Hidden;
            encounterRoutine = null;
        }

        private void Win()
        {
            gameplayEnabled = false;
            ResolveFish(true);

            GameController controller = FindFirstObjectByType<GameController>();
            if (controller != null)
            {
                controller.OnFishCaught();
            }
        }

        private void Lose()
        {
            gameplayEnabled = false;
            ResolveFish(false);

            GameController controller = FindFirstObjectByType<GameController>();
            if (controller != null)
            {
                controller.OnFishEscaped();
            }
        }
    }
}