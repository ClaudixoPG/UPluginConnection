using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Nuevo sistema

namespace SpaceShip
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 5.5f;
        public float fireRate = 0.25f;
        public int lives = 3;
        public int shieldsAmount = 3;
        public float canFire = 0.0f;
        public float shieldDuration = 5.0f;

        public GameObject BulletPref;
        public AudioManager audioManager;
        public AudioSource actualAudio;
        public GameObject shield;
        public int actualWeapon = 0;
        public List<Bullet> bullets;

        public Vector2 moveInput; // Nuevo sistema de entrada

        private void Start()
        {
            shield.SetActive(false);
        }

        void Update()
        {
            Movement();
            CheckBoundaries();

            // Si el escudo está activo, contamos duración
            if (shield.activeSelf)
            {
                shieldDuration -= Time.deltaTime;
                if (shieldDuration < 0)
                {
                    shield.SetActive(false);
                    shieldDuration = 5.0f;
                    GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }

        void Movement()
        {
            Vector3 move = new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime;
            transform.Translate(move);
        }

        public void UseShields()
        {
            if (shieldsAmount > 0 && !shield.activeSelf)
            {
                shieldsAmount--;
                shield.SetActive(true);
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        public void Fire()
        {
            if (Time.time < canFire) return;

            switch (BulletPref.name)
            {
                case "Bullet":
                    Instantiate(BulletPref, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
                    actualAudio.pitch = 1;
                    actualAudio.Play();
                    break;

                case "Missile":
                    var bullet1 = Instantiate(BulletPref, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
                    bullet1.GetComponent<Missile>().direction = Vector2.up;

                    var bullet2 = Instantiate(BulletPref, transform.position + new Vector3(0.5f, 0.8f, 0), Quaternion.identity);
                    bullet2.GetComponent<Missile>().direction = new Vector2(0.5f, 1);

                    var bullet3 = Instantiate(BulletPref, transform.position + new Vector3(-0.5f, 0.8f, 0), Quaternion.identity);
                    bullet3.GetComponent<Missile>().direction = new Vector2(-0.5f, 1);

                    actualAudio.Play();
                    break;

                case "Energy Ball":
                    Instantiate(BulletPref, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
                    actualAudio.pitch = Random.Range(0.5f, 1f);
                    actualAudio.Play();
                    break;
            }

            canFire = Time.time + fireRate;
        }

        public void ChangeWeapon(int weaponIndex)
        {
            if (weaponIndex < bullets.Count)
            {
                BulletPref = bullets[weaponIndex].gameObject;
                actualWeapon = weaponIndex;
            }
        }

        void CheckBoundaries()
        {
            var cam = Camera.main;
            float xMax = cam.orthographicSize * cam.aspect;
            float yMax = cam.orthographicSize;

            if (transform.position.x > xMax)
                transform.position = new Vector3(-xMax, transform.position.y, 0);
            else if (transform.position.x < -xMax)
                transform.position = new Vector3(xMax, transform.position.y, 0);

            if (transform.position.y > yMax)
                transform.position = new Vector3(transform.position.x, -yMax, 0);
            else if (transform.position.y < -yMax)
                transform.position = new Vector3(transform.position.x, yMax, 0);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Destroy(collision.gameObject);
                ChangeShipState();

                if (lives > 1)
                {
                    lives--;
                    Debug.Log("Lives: " + lives);
                }
                else
                {
                    lives--;
                    Destroy(this.gameObject);
                }
            }
        }

        // --- Estado de la nave ---
        public enum ShipState { FullHealth, SlightlyDamaged, Damaged, HeavilyDamaged, Destroyed }
        public ShipState shipState;
        public List<Sprite> shipSprites = new List<Sprite>();

        void ChangeShipState()
        {
            var currentState = shipState;
            var newSprite = shipSprites[(int)currentState];
            GetComponent<SpriteRenderer>().sprite = newSprite;

            switch (currentState)
            {
                case ShipState.FullHealth: shipState = ShipState.SlightlyDamaged; break;
                case ShipState.SlightlyDamaged: shipState = ShipState.Damaged; break;
                case ShipState.Damaged: shipState = ShipState.HeavilyDamaged; break;
                case ShipState.HeavilyDamaged: shipState = ShipState.Destroyed; break;
            }
        }
    }
}