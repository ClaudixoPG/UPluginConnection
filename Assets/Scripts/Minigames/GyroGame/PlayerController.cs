using UnityEngine;

namespace GyroMiniGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float minX = -4f;
        public float maxX = 4f;

        private float _currentInput;

        public void SetInput(float horizontalInput)
        {
            _currentInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        }

        private void Update()
        {
            Vector3 position = transform.position;
            position.x += _currentInput * moveSpeed * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, minX, maxX);
            transform.position = position;
        }
    }
}