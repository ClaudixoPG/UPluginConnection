using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GolfGame
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Ball Settings")]
        [SerializeField] private float maxPower = 10f;
        [SerializeField] private float minPower = 2f;
        [SerializeField] private float maxGoalSpeed = 4f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private LineRenderer lr;
        public float rotationSpeed = 100f;
        //Privadas


        private bool isDragging;
        private bool inHole;
        public bool isTurningLeft;
        public bool isTurningRight;
        public bool isCharging = false;


        private void Update()
        {

            Turning();
            Charging();
            
        }

        public void DragChange(/*Vector2 pos*/)
        {
            isDragging = true;
            lr.positionCount = 2;
            Vector2 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            float distance = Vector2.Distance(transform.position, pos);


            Vector2 dir = (Vector2)transform.position - pos;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, (Vector2)transform.position + Vector2.ClampMagnitude((dir * minPower) / 2, maxPower / 2));
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

        
        public void DragRelease(/*Vector2 pos*/)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            float distance = Vector2.Distance(transform.position, pos);


            //float distance = Vector2.Distance(transform.position, pos);
            isDragging = false;
            lr.positionCount = 0;
            if (distance < 1f)
            {
                return;
            }

            Vector2 dir = (Vector2)transform.position - pos;

            rb.linearVelocity = Vector2.ClampMagnitude(dir * minPower, maxPower);
            //rb.AddForce(Vector2.ClampMagnitude(dir.normalized * minPower, maxPower));

        }

        public void Charging()
        {             
            if(!isCharging) return;

            var initialLaunchForce = 5;
            minPower += initialLaunchForce * Time.deltaTime;
            Debug.Log( "Poder de lanzamiento :" + minPower);
        }
        public void Release()
        {
            isCharging = false;            
            var dir = transform.up;
            rb.linearVelocity = Vector2.ClampMagnitude(dir * minPower, maxPower);
            Debug.Log("Launching" + Vector2.ClampMagnitude(dir * minPower, maxPower));
            
            minPower = 0f;
        }

        public void TurnRight()
        {
            transform.Rotate((Vector3.forward * -rotationSpeed) * Time.deltaTime);
        }
        public void TurnLeft()
        {
            //transform.rotation.eulerAngles = new Vector3(); 
            transform.Rotate((Vector3.forward * rotationSpeed) * Time.deltaTime);
        }
        private void BallInHole()
        {
            Debug.Log("Entro en el hoyo siuuuuuuuuuuu");
            if (inHole)
            {
                return;
            }

            if (rb.linearVelocity.magnitude <= maxGoalSpeed)
            {
                inHole = true;

                rb.linearVelocity = Vector2.zero;
                Debug.Log("LE PEGOOOOOOOO en el hoyo siuuuuuuuuuuu");
                gameObject.SetActive(false);

                //LevelComplete
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Goal"))
            {
                Debug.Log("Choco en el hoyo siuuuuuuuuuuu");
                BallInHole();
            }

            if(collision.CompareTag("Enemy"))
            {
              UnityEngine.SceneManagement.SceneManager.LoadScene("GolfMiniGame2");
                
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            
            if (collision.CompareTag("Goal"))
            {
                Debug.Log("Esta en el hoyo siuuuuuuuuuuu");
                BallInHole();
            }
        }
      
      
    }

}
