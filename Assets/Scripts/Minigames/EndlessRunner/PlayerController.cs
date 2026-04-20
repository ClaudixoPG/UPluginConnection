using UnityEngine;

namespace EndlessRunner
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Jump")]
        [SerializeField] private float jumpHeight = 2.2f;
        [SerializeField] private float jumpDuration = 0.45f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform feetPos;
        [SerializeField] private float groundDistance = 0.25f;

        [Header("Crouch")]
        [SerializeField] private float crouchHight = 0.5f;

        private bool gameplayEnabled = true;
        private bool isJumping = false;
        private bool isGrounded = true;
        private bool isCrouching = false;

        private float jumpTimer = 0f;
        private float groundY;
        private float _crouchOffset = 0.5f;

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
            groundY = transform.position.y;
        }

        private void Update()
        {
            if (!IsGameplayActive) return;

            UpdateGroundState();
            UpdateJumpArc();
        }

        private void UpdateGroundState()
        {
            if (isJumping)
            {
                isGrounded = false;
                return;
            }

            isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);

            if (isGrounded)
            {
                Vector3 pos = transform.position;
                pos.y = groundY;
                transform.position = pos;
            }
        }

        private void UpdateJumpArc()
        {
            if (!isJumping) return;

            jumpTimer += Time.deltaTime;
            float t = jumpTimer / jumpDuration;

            if (t >= 1f)
            {
                EndJump();
                return;
            }

            // Parábola simple: 4h * t * (1 - t)
            float arc = 4f * jumpHeight * t * (1f - t);

            Vector3 pos = transform.position;
            pos.y = groundY + arc;
            transform.position = pos;
        }

        public void Jump()
        {
            if (!IsGameplayActive) return;
            if (isJumping) return;

            bool groundedNow = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);
            if (!groundedNow) return;

            if (isCrouching)
            {
                StandUp();
            }

            isJumping = true;
            isGrounded = false;
            jumpTimer = 0f;
        }

        private void EndJump()
        {
            isJumping = false;
            isGrounded = true;
            jumpTimer = 0f;

            Vector3 pos = transform.position;
            pos.y = groundY;
            transform.position = pos;
        }

        public void CancelJump()
        {
            if (!IsGameplayActive) return;
            // vacío intencionalmente para esta prueba
        }

        public void Crounch()
        {
            if (!IsGameplayActive || isCrouching || isJumping) return;

            bool groundedNow = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);
            if (!groundedNow) return;

            isCrouching = true;
            transform.localScale = new Vector3(_originalScale.x, crouchHight, _originalScale.z);
            transform.position += Vector3.down * _crouchOffset;
        }

        public void StandUp()
        {
            if (!isCrouching) return;

            isCrouching = false;
            transform.localScale = _originalScale;
            transform.position += Vector3.up * _crouchOffset;
        }

        public void SetGameplayEnabled(bool value)
        {
            gameplayEnabled = value;
        }

        public void ResetState()
        {
            isJumping = false;
            isGrounded = true;
            isCrouching = false;
            jumpTimer = 0f;

            transform.localScale = _originalScale;
            transform.position = _originalPosition;
            groundY = _originalPosition.y;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsGameplayActive) return;

            if (collision.gameObject.CompareTag("Obstacle"))
            {
                GameController.Instance.GameOver();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (feetPos == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(feetPos.position, groundDistance);
        }
    }
}