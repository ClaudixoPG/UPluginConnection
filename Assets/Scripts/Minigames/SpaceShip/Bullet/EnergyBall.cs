using UnityEngine;

namespace SpaceShip
{
    public class EnergyBall : Bullet
    {
        public override void Movement()
        {
            transform.Translate(new Vector3(Mathf.Sin(Time.time * 1.5f), 1f, 0f) * speed * Time.deltaTime);
        }
    }
}