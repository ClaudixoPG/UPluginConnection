using UnityEngine;

namespace EndlessRunner
{
    public abstract class Obstacle : MonoBehaviour
    {
        protected float speed = 10f;
        protected Vector2 direction = Vector2.left;

        public float Speed { get => speed; set => speed = value; }
        public Vector2 Direction { get => direction; set => direction = value; }

        public abstract void Spawn(Vector2 position);
        public abstract void Move();
        public abstract void Initialize(Vector2 direction, float speed, Vector2 position);
    }
}