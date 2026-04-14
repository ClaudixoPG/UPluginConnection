using UnityEngine;

namespace SpaceShip
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] protected float speed = 5f;
        [SerializeField] private float lifeTime = 2.5f;
        [SerializeField] private float outOfBoundsMargin = 1.5f;

        protected GameManager gameManager;
        private float lifeTimer;

        protected virtual void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        protected virtual void Update()
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= lifeTime)
            {
                Destroy(gameObject);
                return;
            }

            Movement();
            DestroyIfOutOfBounds();
        }

        public virtual void Movement()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        private void DestroyIfOutOfBounds()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            float xMax = cam.orthographicSize * cam.aspect + outOfBoundsMargin;
            float yMax = cam.orthographicSize + outOfBoundsMargin;
            Vector3 pos = transform.position;

            if (pos.x > xMax || pos.x < -xMax || pos.y > yMax || pos.y < -yMax)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision == null) return;
            if (!collision.gameObject.CompareTag("Enemy")) return;

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.DestroyEnemy();
            }
            else
            {
                Destroy(collision.gameObject);
            }

            if (gameManager != null)
            {
                gameManager.AddScore(10);
            }

            Destroy(gameObject);
        }
    }
}