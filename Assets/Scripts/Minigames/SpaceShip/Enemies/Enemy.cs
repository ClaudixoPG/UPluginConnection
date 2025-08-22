using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class Enemy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Movement();
        }

        public float speed = 10f;
        public int health = 1;
        private GameObject explosionPrefab;

        public void Movement()
        {
            //transform.Translate(Vector3.down * speed * Time.deltaTime);
            //Use Sin and Cos to move the enemy down in a wave pattern
            transform.Translate(new Vector3(Mathf.Sin(Time.time * 1.5f), -1, 0) * speed * Time.deltaTime);
        }

        /*
            public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        protected void Die()
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

         */


        //Test if player collides with enemy, implement on CollisionEnter2D

    }
}