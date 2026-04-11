using UnityEngine;

namespace EndlessRunner
{
    public class Spike : Obstacle
    {
        private bool IsGameplayActive =>
            !GameController.IsGameOver &&
            MinigameContext.IsMeasurementActive;

        private void Update()
        {
            if (!IsGameplayActive) return;
            Move();
        }

        public override void Spawn(Vector2 position)
        {
            var cam = Camera.main;

            if (cam != null)
            {
                Vector3 leftEdge = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
                Vector3 rightEdge = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));

                Vector3 spikePosition = new Vector3(
                    Random.value > 0.5f ? leftEdge.x : rightEdge.x,
                    0f,
                    0f
                );

                var dir = cam.WorldToScreenPoint(spikePosition).x < cam.pixelWidth / 2
                    ? Vector2.right
                    : Vector2.left;

                Spike spikeClone = Instantiate(gameObject, spikePosition, Quaternion.identity).GetComponent<Spike>();
                spikeClone.Initialize(dir, Speed, new Vector2(spikePosition.x, position.y));
            }
        }

        public override void Move()
        {
            transform.Translate(Direction * Speed * Time.deltaTime);
        }

        public override void Initialize(Vector2 direction, float speed, Vector2 position)
        {
            Direction = direction;
            Speed = speed;
            transform.position = position;
        }
    }
}