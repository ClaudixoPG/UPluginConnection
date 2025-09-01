using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GolfGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Ball Settings")]
        [SerializeField] private float maxPower = 10f;
        [SerializeField] private float maxGoalSpeed = 4f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private LineRenderer lr;
        public float rotationSpeed = 100f;

        // Flags de control
        private bool inHole;
        public bool isTurningLeft;
        public bool isTurningRight;
        public bool isCharging = false;
        private bool isDragging; // si decides mantener input por mouse

        // Fuerza normalizada (0–1)
        private float normalizedForce = 0f;

        private void Update()
        {
            Turning();

            // carga local (si no viene del smartwatch)
            if (isCharging)
            {
                Charging();
            }
        }

        public void Charging()
        {
            normalizedForce += Time.deltaTime;
            normalizedForce = Mathf.Clamp01(normalizedForce);
        }

        public void SetNormalizedForce(float value)
        {
            normalizedForce = Mathf.Clamp01(value);
        }

        public void Release()
        {
            float actualPower = normalizedForce * maxPower;
            var force = transform.up * actualPower;
            rb.linearVelocity = Vector2.ClampMagnitude(force, maxPower);

            isCharging = false;
            normalizedForce = 0f; // reset después de lanzar
        }

        private void Turning()
        {
            if (isTurningLeft)
            {
                TurnLeft();
            }
            else if (isTurningRight)
            {
                TurnRight();
            }
        }

        public void TurnRight()
        {
            transform.Rotate((Vector3.forward * -rotationSpeed) * Time.deltaTime);
        }

        public void TurnLeft()
        {
            transform.Rotate((Vector3.forward * rotationSpeed) * Time.deltaTime);
        }

        private void BallInHole()
        {
            Debug.Log("Entro en el hoyo");
            if (inHole) return;

            if (rb.linearVelocity.magnitude <= maxGoalSpeed)
            {
                inHole = true;
                rb.linearVelocity = Vector2.zero;
                Debug.Log("Pelota dentro del hoyo");
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Goal"))
            {
                BallInHole();
            }

            if (collision.CompareTag("Enemy"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GolfMiniGame2");
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Goal"))
            {
                BallInHole();
            }
        }
    }


}
