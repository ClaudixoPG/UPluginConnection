using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<Obstacle> objectsToSpawn;
        [SerializeField] private float spawnInterval = 2f;

        private float timer;

        private bool IsGameplayActive =>
            !GameController.IsGameOver &&
            MinigameContext.IsMeasurementActive;

        private void Update()
        {
            if (!IsGameplayActive) return;
            SpawnLoop();
        }

        private void SpawnLoop()
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
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