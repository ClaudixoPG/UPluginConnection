using UnityEngine;

namespace SpaceShip
{
    public class Enemy : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed = 10f;

        [Header("Destroy FX")]
        [SerializeField] private GameObject destructionPrefab;

        private bool isBeingDestroyed;

        private void Update()
        {
            Movement();
        }

        public void Movement()
        {
            transform.Translate(
                new Vector3(Mathf.Sin(Time.time * 1.5f), -1f, 0f) * speed * Time.deltaTime
            );
        }

        public void DestroyEnemy()
        {
            if (isBeingDestroyed)
                return;

            isBeingDestroyed = true;

            if (destructionPrefab != null)
            {
                Instantiate(destructionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}