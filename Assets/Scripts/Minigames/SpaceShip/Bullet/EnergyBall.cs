using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class EnergyBall : Bullet
    {
        // Update is called once per frame
        void Update()
        {
            Movement();
        }

        //Override the movement method to move the bullet in a wave pattern
        public override void Movement()
        {
            //use sin to move the bullet in a wave pattern
            transform.Translate(new Vector3(Mathf.Sin(Time.time * 1.5f), 1, 0) * speed * Time.deltaTime);
        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision != null)
            {
                Debug.Log("Collided with: " + collision.gameObject.name);

                if (collision.gameObject.CompareTag("Enemy"))
                {
                    //Destroy the enemy
                    gameManager.AddScore(10);
                    Destroy(collision.gameObject);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}