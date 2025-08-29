using UnityEngine;

namespace GolfGame
{
    public class PlayerController : MonoBehaviour
    {


        [Header("Ball Settings")]
        [SerializeField] private float maxPower = 10f;
        [SerializeField] private float minPower = 2f;
        [SerializeField] private float maxGoalSpeed = 4f;



        //Privadas
        private Rigidbody2D rb;
        private LineRenderer lr;
        private bool isDragging;
        private bool inHole;


        private void Start()
        {

        }

        private void Update()
        {
            PlayerLogic();
        }


        private void PlayerLogic()
        {
            BallInput();
        }

        private void BallInput()
        {
            Vector2 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector2.Distance(transform.position, inputPos);

            if(Input.GetMouseButtonDown(0) && distance <= 0.5f) 
            {
                DragStart();
            }

            if(Input.GetMouseButtonDown(0) && isDragging)
            {
                DragChange();
            }

            if(Input.GetMouseButtonUp(0) && isDragging)
            {
                DragRelease(inputPos);
            }
        }

        private void DragStart()
        {
            isDragging = true;
        }

        private void DragChange()
        {

        }

        private void DragRelease(Vector2 pos)
        {
            float distance = Vector2.Distance(transform.position, pos);
            isDragging = false;

            if()
            {

            }
        }
    }

}
