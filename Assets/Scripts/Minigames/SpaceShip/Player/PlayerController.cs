using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed = 5.5f;

        [Header("Combat")]
        [SerializeField] private float fireRate = 0.45f;
        [SerializeField] private List<Bullet> bullets;
        [SerializeField] private AudioSource shotAudio;

        [Header("Stats")]
        [SerializeField] private int maxLives = 3;
        [SerializeField] private int maxShields = 3;

        [Header("Visual")]
        [SerializeField] private GameObject shield;
        [SerializeField] private List<Sprite> shipSprites = new List<Sprite>();

        public Vector2 moveInput;

        private bool gameplayEnabled;
        private float nextFireTime;

        private Vector3 startPosition;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;

        private Bullet currentBulletPrefab;
        private int currentWeaponIndex;

        public int Lives { get; private set; }
        public int ShieldsAmount { get; private set; }

        public string CurrentWeaponName =>
            currentBulletPrefab != null ? currentBulletPrefab.name : "None";

        public enum ShipState
        {
            FullHealth,
            SlightlyDamaged,
            Damaged,
            HeavilyDamaged,
            Destroyed
        }

        public ShipState shipState;

        private void Awake()
        {
            startPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (shield != null)
                shield.SetActive(false);

            ResetShip();
        }

        private void Update()
        {
            if (!gameplayEnabled) return;

            Movement();
            CheckBoundaries();
            AutoFire();
        }

        public void SetGameplayEnabled(bool value)
        {
            gameplayEnabled = value;
        }

        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        public void ResetShip()
        {
            gameObject.SetActive(true);

            transform.position = startPosition;

            Lives = maxLives;
            ShieldsAmount = maxShields;

            shipState = ShipState.FullHealth;
            ApplyShipSprite();

            moveInput = Vector2.zero;
            nextFireTime = 0f;

            if (shield != null)
                shield.SetActive(false);

            if (boxCollider != null)
                boxCollider.enabled = true;

            if (bullets != null && bullets.Count > 0)
            {
                ChangeWeapon(0);
            }
        }

        private void Movement()
        {
            Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f) * speed * Time.deltaTime;
            transform.Translate(move);
        }

        private void AutoFire()
        {
            if (Time.time < nextFireTime) return;
            if (currentBulletPrefab == null) return;

            Fire();
            nextFireTime = Time.time + fireRate;
        }

        public void Fire()
        {
            if (currentBulletPrefab == null) return;

            Vector3 spawnBase = transform.position + new Vector3(0f, 0.8f, 0f);

            switch (currentBulletPrefab.name)
            {
                case "Bullet":
                    Instantiate(currentBulletPrefab.gameObject, spawnBase, Quaternion.identity);
                    break;

                case "Missile":
                    Missile bullet1 = Instantiate(
                        currentBulletPrefab.gameObject,
                        spawnBase,
                        Quaternion.identity
                    ).GetComponent<Missile>();
                    bullet1.direction = Vector2.up;

                    Missile bullet2 = Instantiate(
                        currentBulletPrefab.gameObject,
                        transform.position + new Vector3(0.5f, 0.8f, 0f),
                        Quaternion.identity
                    ).GetComponent<Missile>();
                    bullet2.direction = new Vector2(0.5f, 1f);

                    Missile bullet3 = Instantiate(
                        currentBulletPrefab.gameObject,
                        transform.position + new Vector3(-0.5f, 0.8f, 0f),
                        Quaternion.identity
                    ).GetComponent<Missile>();
                    bullet3.direction = new Vector2(-0.5f, 1f);
                    break;

                case "Energy Ball":
                    Instantiate(currentBulletPrefab.gameObject, spawnBase, Quaternion.identity);
                    break;

                default:
                    Instantiate(currentBulletPrefab.gameObject, spawnBase, Quaternion.identity);
                    break;
            }

            if (shotAudio != null)
            {
                shotAudio.pitch = currentBulletPrefab.name == "Energy Ball"
                    ? Random.Range(0.5f, 1f)
                    : 1f;
                shotAudio.Play();
            }
        }

        public void ChangeWeapon(int weaponIndex)
        {
            if (bullets == null || bullets.Count == 0) return;
            if (weaponIndex < 0 || weaponIndex >= bullets.Count) return;

            currentBulletPrefab = bullets[weaponIndex];
            currentWeaponIndex = weaponIndex;

            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.SetWeaponIcon(currentWeaponIndex);
            }
        }

        public void ChangeToRandomWeapon()
        {
            if (bullets == null || bullets.Count == 0) return;

            int randomIndex = Random.Range(0, bullets.Count);
            ChangeWeapon(randomIndex);
        }

        private void CheckBoundaries()
        {
            var cam = Camera.main;
            if (cam == null) return;

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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!gameplayEnabled) return;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.DestroyEnemy();
                }
                else
                {
                    Destroy(collision.gameObject);
                }

                TakeDamage(1);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!gameplayEnabled) return;

            WeaponPowerUp powerUp = other.GetComponent<WeaponPowerUp>();
            if (powerUp != null)
            {
                ChangeWeapon(powerUp.WeaponIndex);
                Destroy(powerUp.gameObject);
            }
        }

        private void TakeDamage(int damage)
        {
            Lives -= damage;
            UpdateShipState();

            if (Lives <= 0)
            {
                Lives = 0;
                gameplayEnabled = false;
                gameObject.SetActive(false);

                if (GameController.Instance != null)
                {
                    GameController.Instance.OnPlayerDied();
                }
            }
        }

        private void UpdateShipState()
        {
            switch (Lives)
            {
                case 3:
                    shipState = ShipState.FullHealth;
                    break;
                case 2:
                    shipState = ShipState.SlightlyDamaged;
                    break;
                case 1:
                    shipState = ShipState.Damaged;
                    break;
                case 0:
                    shipState = ShipState.Destroyed;
                    break;
                default:
                    shipState = ShipState.HeavilyDamaged;
                    break;
            }

            ApplyShipSprite();
        }

        private void ApplyShipSprite()
        {
            if (spriteRenderer == null) return;
            if (shipSprites == null || shipSprites.Count == 0) return;

            int index = Mathf.Clamp((int)shipState, 0, shipSprites.Count - 1);
            spriteRenderer.sprite = shipSprites[index];
        }
    }
}