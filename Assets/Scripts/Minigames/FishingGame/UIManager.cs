using TMPro;
using UnityEngine;

namespace FishingGame
{
    public class UIManager : MonoBehaviour
    {
        [Header("Top HUD")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI currentFishRarityText;
        [SerializeField] private TextMeshProUGUI normalCaughtText;
        [SerializeField] private TextMeshProUGUI rareCaughtText;
        [SerializeField] private TextMeshProUGUI legendaryCaughtText;

        [Header("Catch / Escape Feedback")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackTitleText;
        [SerializeField] private TextMeshProUGUI feedbackSubtitleText;
        [SerializeField] private TMPWavyText feedbackWavyText;

        [Header("Final Results")]
        [SerializeField] private GameObject finalResultPanel;
        [SerializeField] private TextMeshProUGUI finalTitleText;
        [SerializeField] private TextMeshProUGUI finalNormalText;
        [SerializeField] private TextMeshProUGUI finalRareText;
        [SerializeField] private TextMeshProUGUI finalLegendaryText;
        [SerializeField] private TextMeshProUGUI finalEscapedText;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        private void Start()
        {
            HideFeedback();
            HideFinalResults();
            UpdateTimer(280f);
            UpdateCurrentFishRarity(GameController.FishRarity.Normal);
            UpdateFishCounters(0, 0, 0);
        }

        public void UpdateTimer(float remainingSeconds)
        {
            if (timerText == null) return;

            int totalSeconds = Mathf.CeilToInt(remainingSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        public void UpdateCurrentFishRarity(GameController.FishRarity rarity)
        {
            if (currentFishRarityText == null) return;

            switch (rarity)
            {
                case GameController.FishRarity.Normal:
                    currentFishRarityText.text = "Current Fish: Normal";
                    break;
                case GameController.FishRarity.Rare:
                    currentFishRarityText.text = "Current Fish: Rare";
                    break;
                case GameController.FishRarity.Legendary:
                    currentFishRarityText.text = "Current Fish: Legendary";
                    break;
            }
        }

        public void UpdateFishCounters(int normal, int rare, int legendary)
        {
            if (normalCaughtText != null)
                normalCaughtText.text = $"Normal: {normal}";

            if (rareCaughtText != null)
                rareCaughtText.text = $"Rare: {rare}";

            if (legendaryCaughtText != null)
                legendaryCaughtText.text = $"Legendary: {legendary}";
        }

        public void ShowCatchFeedback(GameController.FishRarity rarity)
        {
            if (feedbackPanel != null)
                feedbackPanel.SetActive(true);

            if (feedbackTitleText != null)
                feedbackTitleText.text = "Well done!";

            if (feedbackSubtitleText != null)
                feedbackSubtitleText.text = GetRarityLabel(rarity);

            if (feedbackWavyText != null)
                feedbackWavyText.RefreshNow();
        }

        public void ShowEscapeFeedback(GameController.FishRarity rarity)
        {
            if (feedbackPanel != null)
                feedbackPanel.SetActive(true);

            if (feedbackTitleText != null)
                feedbackTitleText.text = "Better luck next time";

            if (feedbackSubtitleText != null)
                feedbackSubtitleText.text = $"{GetRarityLabel(rarity)} escaped";

            if (feedbackWavyText != null)
                feedbackWavyText.RefreshNow();
        }

        public void HideFeedback()
        {
            if (feedbackPanel != null)
                feedbackPanel.SetActive(false);
        }

        public void ShowFinalResults(
            int normalCaught,
            int rareCaught,
            int legendaryCaught,
            int normalEscaped,
            int rareEscaped,
            int legendaryEscaped,
            int totalScore)
        {
            if (finalResultPanel != null)
                finalResultPanel.SetActive(true);

            if (finalTitleText != null)
                finalTitleText.text = "Fishing Results";

            if (finalNormalText != null)
                finalNormalText.text = $"Normal Caught: {normalCaught}";

            if (finalRareText != null)
                finalRareText.text = $"Rare Caught: {rareCaught}";

            if (finalLegendaryText != null)
                finalLegendaryText.text = $"Legendary Caught: {legendaryCaught}";

            if (finalEscapedText != null)
            {
                int totalEscaped = normalEscaped + rareEscaped + legendaryEscaped;
                finalEscapedText.text = $"Escaped: {totalEscaped}";
            }

            if (finalScoreText != null)
                finalScoreText.text = $"Total Score: {totalScore}";
        }

        public void HideFinalResults()
        {
            if (finalResultPanel != null)
                finalResultPanel.SetActive(false);
        }

        private string GetRarityLabel(GameController.FishRarity rarity)
        {
            switch (rarity)
            {
                case GameController.FishRarity.Normal:
                    return "Normal Fish";
                case GameController.FishRarity.Rare:
                    return "Rare Fish";
                case GameController.FishRarity.Legendary:
                    return "Legendary Fish";
                default:
                    return "Fish";
            }
        }
    }
}