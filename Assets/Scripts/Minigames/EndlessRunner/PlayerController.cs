using UnityEngine;

namespace EndlessRunner
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform feetPos;
        [SerializeField] private float groundDistance = 0.25f;
        [SerializeField] private float jumpoTime = 0.3f;

        //crouch
        [SerializeField] private float crouchHight = 0.5f;

        private bool isGrounded;
        private bool isJumping;
        private float jumpTimer;

        private void Update()
        {
            // Solo verificamos el estado del suelo
            isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);

            if (isGrounded && !isJumping)
            {
                jumpTimer = 0f;
            }

            // Si estamos saltando y el tiempo extra sigue corriendo
            if (isJumping && jumpTimer > 0f)
            {
                rb.AddForce(new Vector2(0, jumpForce * Time.deltaTime), ForceMode2D.Impulse);
                jumpTimer -= Time.deltaTime;
            }
        }

        // --- Métodos públicos llamados desde GameController ---
        public void Jump()
        {
            if (isGrounded)
            {
                isJumping = true;
                jumpTimer = jumpoTime;
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }

        public void CancelJump()
        {
            isJumping = false;
        }

        public void Crounch()
        {
            if (isGrounded)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchHight, transform.localScale.z);
                //move player position down
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            }
        }
        public void StandUp()
        {
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
            //move player position up
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                // Aquí puedes manejar lo que sucede cuando el jugador choca con un obstáculo
                Debug.Log("Game Over!");
                Destroy(gameObject);
                GameController.Instance.GameOver();
            }
        }
    }
}
