using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private WeaponPowerUp[] weaponPowerUpPrefabs;
        [SerializeField] private PlayerController player;

        [Header("Spawn")]
        [SerializeField] private float enemySpawnTime = 1.5f;
        [SerializeField] private float powerUpSpawnTime = 6f;
        [SerializeField] private float spawnXMin = -8f;
        [SerializeField] private float spawnXMax = 8f;
        [SerializeField] private float spawnY = 7f;

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI liveText;
        [SerializeField] private TextMeshProUGUI shieldsText;
        [SerializeField] private TextMeshProUGUI weaponText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Weapon UI")]
        [SerializeField] private Image bulletImage;
        [SerializeField] private Sprite[] bulletSprites;

        private float enemySpawnTimer;
        private float powerUpSpawnTimer;
        private bool gameplayEnabled;

        public int Score { get; private set; }
        public float ElapsedTime { get; private set; }

        private void Update()
        {
            UpdateHUD();

            if (!gameplayEnabled)
                return;

            ElapsedTime += Time.deltaTime;
            enemySpawnTimer += Time.deltaTime;
            powerUpSpawnTimer += Time.deltaTime;

            TrySpawnEnemy();
            TrySpawnWeaponPowerUp();
        }

        public void SetGameplayEnabled(bool value)
        {
            gameplayEnabled = value;
        }

        public void ResetRun()
        {
            Score = 0;
            ElapsedTime = 0f;
            enemySpawnTimer = 0f;
            powerUpSpawnTimer = 0f;

            if (player != null)
            {
                SetWeaponIcon(0);
            }
        }

        private void UpdateHUD()
        {
            if (player != null)
            {
                if (liveText != null)
                    liveText.text = "Life: " + player.Lives;

                if (shieldsText != null)
                    shieldsText.text = "Shields: " + player.ShieldsAmount;

                if (weaponText != null)
                    weaponText.text = player.CurrentWeaponName;
            }

            if (scoreText != null)
                scoreText.text = "Score: " + Score.ToString();

            if (timeText != null)
                timeText.text = "Time: " + ElapsedTime.ToString("F0");
        }

        private void TrySpawnEnemy()
        {
            if (enemyPrefab == null) return;
            if (enemySpawnTimer < enemySpawnTime) return;

            Instantiate(
                enemyPrefab,
                new Vector3(Random.Range(spawnXMin, spawnXMax), spawnY, 0f),
                Quaternion.identity
            );

            enemySpawnTimer = 0f;
        }

        private void TrySpawnWeaponPowerUp()
        {
            if (weaponPowerUpPrefabs == null || weaponPowerUpPrefabs.Length == 0) return;
            if (powerUpSpawnTimer < powerUpSpawnTime) return;

            int randomIndex = Random.Range(0, weaponPowerUpPrefabs.Length);

            WeaponPowerUp powerUp = Instantiate(
                weaponPowerUpPrefabs[randomIndex],
                new Vector3(Random.Range(spawnXMin, spawnXMax), spawnY, 0f),
                Quaternion.identity
            );

            powerUp.Configure(randomIndex, bulletSprites);

            powerUpSpawnTimer = 0f;
        }

        public void AddScore(int value)
        {
            Score += value;
        }

        public void SetWeaponIcon(int index)
        {
            if (bulletImage == null) return;
            if (bulletSprites == null || bulletSprites.Length == 0) return;
            if (index < 0 || index >= bulletSprites.Length) return;

            bulletImage.sprite = bulletSprites[index];
        }
    }
}