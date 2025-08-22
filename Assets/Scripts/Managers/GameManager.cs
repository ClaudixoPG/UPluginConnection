using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip
{
    public class GameManager : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public float spawnTime = 1.5f;
        public float time = 0.0f;
        public PlayerController player;

        [Header("TEXTOS")]
        public TextMeshProUGUI liveText;
        public TextMeshProUGUI shieldsText;
        public TextMeshProUGUI weaponText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timeText;

        public float TotalTime = 0.0f;
        public int score = 0;


        // Update is called once per frame
        void Update()
        {
            CreateEnemy();
            UpdateCanvas();
            ChangeBulletImage(player.actualWeapon);
            TotalTime += Time.deltaTime;
        }

        void UpdateCanvas()
        {
            liveText.text = "Life: " + player.lives;
            shieldsText.text = "Shields: " + player.shieldsAmount;
            //weaponText.text = "Weapon: " + player.BulletPref.name;
            weaponText.text = player.BulletPref.name;
            scoreText.text = "Score: " + score.ToString();
            //truncate the time to no show decimals
            timeText.text = "Time: " + TotalTime.ToString("F0");
        }

        private void CreateEnemy()
        {
            time += Time.deltaTime;
            if (time > spawnTime)
            {
                Instantiate(enemyPrefab, new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0), Quaternion.identity);
                time = 0.0f;
            }
        }
        public void AddScore(int value)
        {
            score += value;
        }


        [Header("UI")]
        public Image bulletImage;
        public List<Sprite> bulletSprites;

        public void ChangeBulletImage(int index)
        {
            Debug.Log("ChangeBulletImage: " + index);
            bulletImage.sprite = bulletSprites[index];
        }
    }
}