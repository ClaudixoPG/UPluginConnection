using TMPro;
using UnityEngine;

namespace SpaceShip
{
    public class UIManager : MonoBehaviour
    {
        [Header("Lose Panel")]
        [SerializeField] private GameObject losePanel;
        [SerializeField] private TextMeshProUGUI loseTitleText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI finalTimeText;
        [SerializeField] private TextMeshProUGUI finalLivesText;
        [SerializeField] private TextMeshProUGUI finalWeaponText;

        private void Start()
        {
            HideLoseScreen();
        }

        public void ShowLoseScreen(int finalScore, float survivedTime, int finalLives, string weaponName)
        {
            if (losePanel != null)
                losePanel.SetActive(true);

            if (loseTitleText != null)
                loseTitleText.text = "Mission Failed";

            if (finalScoreText != null)
                finalScoreText.text = $"Score: {finalScore}";

            if (finalTimeText != null)
                finalTimeText.text = $"Time: {survivedTime:F0}";

            if (finalLivesText != null)
                finalLivesText.text = $"Lives: {finalLives}";

            if (finalWeaponText != null)
                finalWeaponText.text = $"Weapon: {weaponName}";
        }

        public void HideLoseScreen()
        {
            if (losePanel != null)
                losePanel.SetActive(false);
        }
    }
}