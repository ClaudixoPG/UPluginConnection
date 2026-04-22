using UnityEngine;
using System.Collections;
using System.Globalization;

namespace FishingGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        private PlayerInputActions inputActions;

        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private UIManager uiManager;

        [Header("Session")]
        [SerializeField] private float sessionDurationSeconds = 280f; // 4:40

        [Header("Fish rarity probabilities")]
        [SerializeField, Range(0f, 1f)] private float normalChance = 0.70f;
        [SerializeField, Range(0f, 1f)] private float rareChance = 0.25f;
        [SerializeField, Range(0f, 1f)] private float legendaryChance = 0.05f;

        public enum GameState
        {
            Waiting,
            Playing,
            Results
        }

        public enum FishRarity
        {
            Normal,
            Rare,
            Legendary
        }

        private Coroutine nextFishRoutine;
        [SerializeField] private float nextFishDelay = 0.8f;

        public GameState CurrentState { get; private set; } = GameState.Waiting;
        public FishRarity CurrentFishRarity { get; private set; } = FishRarity.Normal;

        public int normalCaught { get; private set; }
        public int rareCaught { get; private set; }
        public int legendaryCaught { get; private set; }

        public int normalEscaped { get; private set; }
        public int rareEscaped { get; private set; }
        public int legendaryEscaped { get; private set; }

        public int TotalScore =>
            (normalCaught * 1) +
            (rareCaught * 2) +
            (legendaryCaught * 3);

        private float remainingTime;
        private bool roundResolved;

        private bool IsGameplayActive =>
            CurrentState == GameState.Playing &&
            MinigameContext.IsMeasurementActive;

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.FishingGame.Enable();

            inputActions.FishingGame.PressScreen.started += ctx =>
            {
                if (IsGameplayActive && playerController != null)
                    playerController.SetHoldInput(1f);
            };

            inputActions.FishingGame.PressScreen.canceled += ctx =>
            {
                if (playerController != null)
                    playerController.SetHoldInput(0f);
            };
        }

        private void Start()
        {
            ResetSession();

            if (uiManager != null)
            {
                uiManager.HideFinalResults();
                uiManager.UpdateTimer(remainingTime);
                uiManager.UpdateCurrentFishRarity(CurrentFishRarity);
                uiManager.UpdateFishCounters(normalCaught, rareCaught, legendaryCaught);
            }

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.ResetRound();
            }
        }

        private void OnEnable() => inputActions.Enable();

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void Update()
        {
            if (CurrentState == GameState.Waiting && MinigameContext.IsMeasurementActive)
            {
                StartSession();
            }

            if (!IsGameplayActive)
                return;

            remainingTime -= Time.deltaTime;
            if (remainingTime < 0f)
                remainingTime = 0f;

            if (uiManager != null)
            {
                uiManager.UpdateTimer(remainingTime);
            }

            if (remainingTime <= 0f)
            {
                EndSession();
            }
        }

        public void HandleMessage(string message)
        {
            if (!IsGameplayActive || playerController == null || string.IsNullOrEmpty(message))
                return;

            if (message.StartsWith("Hold:"))
            {
                ReadHoldValue(message, 5);
                return;
            }

            /*if (message.StartsWith("Time:"))
            {
                ReadHoldValue(message, 5);
            }*/
        }

        private void ReadHoldValue(string message, int prefixLength)
        {
            int commaIndex = message.IndexOf(',', prefixLength);
            string valueText = commaIndex >= 0
                ? message.Substring(prefixLength, commaIndex - prefixLength)
                : message.Substring(prefixLength);

            if (float.TryParse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture, out float holdValue))
            {
                playerController.SetHoldInput(holdValue);
            }
        }

        private void StartSession()
        {
            CurrentState = GameState.Playing;
            roundResolved = false;

            if (uiManager != null)
            {
                uiManager.HideFinalResults();
                uiManager.UpdateTimer(remainingTime);
                uiManager.UpdateFishCounters(normalCaught, rareCaught, legendaryCaught);
            }

            StartNextFish();
        }

        private void EndSession()
        {
            CurrentState = GameState.Results;

            if (playerController != null)
            {
                playerController.SetGameplayEnabled(false);
                playerController.SetHoldInput(0f);
            }

            if (uiManager != null)
            {
                uiManager.ShowFinalResults(
                    normalCaught,
                    rareCaught,
                    legendaryCaught,
                    normalEscaped,
                    rareEscaped,
                    legendaryEscaped,
                    TotalScore
                );
            }
        }

        private void StartNextFish()
        {
            if (CurrentState != GameState.Playing || playerController == null)
                return;

            roundResolved = false;
            CurrentFishRarity = RollFishRarity();

            playerController.ConfigureFish(CurrentFishRarity);
            playerController.ResetRound();
            playerController.SetHoldInput(0f);
            playerController.BeginFishEncounter();

            if (uiManager != null)
            {
                uiManager.UpdateCurrentFishRarity(CurrentFishRarity);
            }
        }

        public void OnFishCaught()
        {
            if (!IsGameplayActive || roundResolved) return;
            roundResolved = true;

            switch (CurrentFishRarity)
            {
                case FishRarity.Normal: normalCaught++; break;
                case FishRarity.Rare: rareCaught++; break;
                case FishRarity.Legendary: legendaryCaught++; break;
            }

            if (uiManager != null)
            {
                uiManager.UpdateFishCounters(normalCaught, rareCaught, legendaryCaught);
                uiManager.ShowCatchFeedback(CurrentFishRarity);
            }

            QueueNextFish();
        }

        public void OnFishEscaped()
        {
            if (!IsGameplayActive || roundResolved) return;
            roundResolved = true;

            switch (CurrentFishRarity)
            {
                case FishRarity.Normal: normalEscaped++; break;
                case FishRarity.Rare: rareEscaped++; break;
                case FishRarity.Legendary: legendaryEscaped++; break;
            }

            if (uiManager != null)
            {
                uiManager.ShowEscapeFeedback(CurrentFishRarity);
            }

            QueueNextFish();
        }

        private void QueueNextFish()
        {
            if (nextFishRoutine != null)
                StopCoroutine(nextFishRoutine);

            nextFishRoutine = StartCoroutine(NextFishRoutine());
        }

        private IEnumerator NextFishRoutine()
        {
            yield return new WaitForSeconds(nextFishDelay);

            if (uiManager != null)
            {
                uiManager.HideFeedback();
            }

            if (IsGameplayActive)
                StartNextFish();

            nextFishRoutine = null;
        }

        private FishRarity RollFishRarity()
        {
            float total = normalChance + rareChance + legendaryChance;
            if (total <= 0f)
                return FishRarity.Normal;

            float value = Random.value * total;

            if (value < normalChance)
                return FishRarity.Normal;

            value -= normalChance;
            if (value < rareChance)
                return FishRarity.Rare;

            return FishRarity.Legendary;
        }

        private void ResetSession()
        {
            CurrentState = GameState.Waiting;
            CurrentFishRarity = FishRarity.Normal;

            remainingTime = sessionDurationSeconds;

            normalCaught = 0;
            rareCaught = 0;
            legendaryCaught = 0;

            normalEscaped = 0;
            rareEscaped = 0;
            legendaryEscaped = 0;

            roundResolved = false;
        }
    }
}