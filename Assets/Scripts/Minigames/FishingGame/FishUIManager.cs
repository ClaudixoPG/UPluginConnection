using TMPro;
using UnityEngine;

namespace FishingGame
{
    public class FishingUIManager : MonoBehaviour
    {
        [Header("Counters")]
        [SerializeField] private TextMeshProUGUI fishCaughtText;
        [SerializeField] private TextMeshProUGUI fishMissedText;

        [Header("Round Result")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI resultTitleText;
        [SerializeField] private TextMeshProUGUI resultSummaryText;

        private void Start()
        {
            HideResultPanel();
            UpdateFishCounter(0, 0);
        }

        public void UpdateFishCounter(int caught, int missed)
        {
            if (fishCaughtText != null)
                fishCaughtText.text = $"Caught: {caught}";

            if (fishMissedText != null)
                fishMissedText.text = $"Escaped: {missed}";
        }

        public void ShowResultPanel(string title, int caught, int missed)
        {
            if (resultPanel != null)
                resultPanel.SetActive(true);

            if (resultTitleText != null)
                resultTitleText.text = title;

            if (resultSummaryText != null)
                resultSummaryText.text = $"Caught: {caught}\nEscaped: {missed}";
        }

        public void HideResultPanel()
        {
            if (resultPanel != null)
                resultPanel.SetActive(false);
        }
    }
}