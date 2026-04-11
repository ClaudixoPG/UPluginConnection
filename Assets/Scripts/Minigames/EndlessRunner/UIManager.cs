using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessRunner
{
    public class UIManager : MonoBehaviour
    {
        [Header("Score UI")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Lose UI")]
        [SerializeField] private GameObject losePanel;
        [SerializeField] private TextMeshProUGUI loseTitleText;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        private void Awake()
        {
            HideLoseScreen();
            UpdateScore(0f);
        }

        public void UpdateScore(float score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {Mathf.RoundToInt(score)}";
            }
        }

        public void ShowLoseScreen(float finalScore)
        {
            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }

            if (loseTitleText != null)
            {
                loseTitleText.text = "Lose";
            }

            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score: {Mathf.RoundToInt(finalScore)}";
            }
        }

        public void HideLoseScreen()
        {
            if (losePanel != null)
            {
                losePanel.SetActive(false);
            }
        }
    }
}