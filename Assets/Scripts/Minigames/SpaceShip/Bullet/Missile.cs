using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceShip
{
    public class Missile : Bullet
    {
        public Vector2 direction;

        // Update is called once per frame
        void Update()
        {
            Movement();
        }

        //Override the movement method to move the bullet in a wave pattern
        public override void Movement()
        {
            //use direction to move the bullet in a straight line pattern
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}