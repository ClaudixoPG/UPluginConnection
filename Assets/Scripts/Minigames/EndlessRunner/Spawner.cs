using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<Obstacle> objectsToSpawn;
        [SerializeField] private float spawnInterval = 2f;

        private float timer;


        // Update is called once per frame
        void Update()
        {
            if (GameController.IsGameOver) return;
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

        void Spawn()
        {
            if (objectsToSpawn.Count == 0) return;

            int index = Random.Range(0, objectsToSpawn.Count);
            //var obj = objectsToSpawn[index];
            objectsToSpawn[index].Spawn(transform.position);
        }
    }
}
