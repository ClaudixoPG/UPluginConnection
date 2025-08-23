using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class Bullet : MonoBehaviour
    {
        public GameManager gameManager;
        public float speed = 5f;

        private void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            Movement();
        }

        /*void Movement()
        {
            transform.Translate(Vector3.up * 5.0f * Time.deltaTime);
        }*/

        public virtual void Movement()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
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
