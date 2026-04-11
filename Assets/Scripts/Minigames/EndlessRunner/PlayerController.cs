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
        [SerializeField] private float crouchHight = 0.5f;

        private bool isGrounded;
        private bool isJumping;
        private float jumpTimer;
        private bool gameplayEnabled = true;

        private Vector3 _originalScale;
        private Vector3 _originalPosition;

        private bool IsGameplayActive =>
            gameplayEnabled &&
            !GameController.IsGameOver &&
            MinigameContext.IsMeasurementActive;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _originalPosition = transform.position;
        }

        private void Update()
        {
            if (!IsGameplayActive) return;

            isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);

            if (isGrounded && !isJumping)
            {
                jumpTimer = 0f;
            }

            if (isJumping && jumpTimer > 0f)
            {
                rb.AddForce(new Vector2(0, jumpForce * Time.deltaTime), ForceMode2D.Impulse);
                jumpTimer -= Time.deltaTime;
            }
        }

        public void Jump()
        {
            if (!IsGameplayActive) return;

            if (isGrounded)
            {
                isJumping = true;
                jumpTimer = jumpoTime;
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }

        public void CancelJump()
        {
            if (!IsGameplayActive) return;
            isJumping = false;
        }

        public void Crounch()
        {
            if (!IsGameplayActive) return;

            if (isGrounded)
            {
                transform.localScale = new Vector3(_originalScale.x, crouchHight, _originalScale.z);
                transform.position = new Vector3(transform.position.x, _originalPosition.y - 0.5f, transform.position.z);
            }
        }

        public void StandUp()
        {
            transform.localScale = _originalScale;
            transform.position = new Vector3(transform.position.x, _originalPosition.y, transform.position.z);
        }

        public void SetGameplayEnabled(bool value)
        {
            gameplayEnabled = value;
        }

        public void ResetState()
        {
            isJumping = false;
            jumpTimer = 0f;
            transform.localScale = _originalScale;
            transform.position = _originalPosition;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.Sleep();
                rb.WakeUp();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsGameplayActive) return;

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                GameController.Instance.GameOver();
            }
        }
    }
}