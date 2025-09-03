using UnityEngine;
namespace EndlessRunner
{
    public class Spike : Obstacle
    {
        private void Update()
        {
            Move();
        }
        public override void Spawn(Vector2 position)
        {
            //get Camera width and height
            var cam = Camera.main;

            //use camera to get world position of screen edges, and set spike position to left or right edge of screen at y = 0
            if (cam != null)
            {
                Vector3 leftEdge = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
                Vector3 rightEdge = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));
                Vector3 spikePosition = new Vector3(Random.value > 0.5f ? leftEdge.x : rightEdge.x, 0f, 0f);
                var dir = cam.WorldToScreenPoint(spikePosition).x < cam.pixelWidth / 2 ? Vector2.right : Vector2.left;
                Spike spikeClone = Instantiate(gameObject, spikePosition, Quaternion.identity).GetComponent<Spike>();
                spikeClone.Initialize(dir, speed,new Vector2(spikePosition.x, position.y));
                return;
            }
        }

        public override void Move()
        {
            //Debug.Log(Direction);
            transform.Translate(direction * Speed * Time.deltaTime);
        }

        public override void Initialize(Vector2 direction, float speed, Vector2 position)
        {
            Direction = direction;
            Speed = speed;
            transform.position = position;
        }
    }
}
