using UnityEngine;
using UnityEngine.UIElements;
namespace EndlessRunner
{
    public class Projectile : Obstacle
    {
        public override void Initialize(Vector2 direction, float speed,Vector2 position)
        {
            Direction = direction;
            Speed = speed;
        }

        public override void Move()
        {
            throw new System.NotImplementedException();
        }

        public override void Spawn(Vector2 position)
        {
            throw new System.NotImplementedException();
        }
    }
}