using UnityEngine;

namespace SpaceShip
{
    public class Missile : Bullet
    {
        public Vector2 direction = Vector2.up;

        public override void Movement()
        {
            transform.Translate(direction.normalized * speed * Time.deltaTime);
        }
    }
}