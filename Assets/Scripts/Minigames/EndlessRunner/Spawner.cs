using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<Obstacle> objectsToSpawn;
        [SerializeField] private float baseSpawnInterval = 2f;
        [SerializeField] private float minSpawnInterval = 0.8f;
        [SerializeField] private float spawnAcceleration = 0.02f;

        private float currentSpawnInterval;

        private float timer;

        private bool IsGameplayActive =>
            !GameController.IsGameOver &&
            MinigameContext.IsMeasurementActive;

        private void Start()
        {
            currentSpawnInterval = baseSpawnInterval;
        }

        private void Update()
        {
            if (!IsGameplayActive) return;
            currentSpawnInterval = Mathf.Max(
                minSpawnInterval,
                currentSpawnInterval - spawnAcceleration * Time.deltaTime
            );
            SpawnLoop();
        }

        private void SpawnLoop()
        {
            timer += Time.deltaTime;
            if (timer >= currentSpawnInterval)
            {
                Spawn();
                timer = 0f;
            }
        }

        private void Spawn()
        {
            if (objectsToSpawn.Count == 0) return;

            int index = Random.Range(0, objectsToSpawn.Count);
            Obstacle selectedObstacle = objectsToSpawn[index];

            if (GameController.Instance != null)
            {
                selectedObstacle.Speed = GameController.Instance.CurrentObstacleSpeed;
            }

            selectedObstacle.Spawn(transform.position);
        }

        public void ResetSpawner()
        {
            timer = 0f;
        }
    }
}