#if UNITY_ANDROID || UNITY_IOS
#define MOBILE
#endif

using UnityEngine;

namespace SpaceShip
{
    public class CameraController : MonoBehaviour
    {
        public float rotationSpeed = 100f; // Speed of camera rotation
        public float scrollSpeed = 10f; // Speed of camera zoom

        private GameObject player; // Reference to the player object
        private Vector3 distanceFromPlayer; // Distance from the player

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = GameObject.Find("Player"); // Find the player object in the scene
            if (player == null)
            {
                Debug.LogError("Player object not found in the scene.");
                return;
            }
            // Set the initial distance from the player
            distanceFromPlayer = transform.position - player.transform.position;

        }

        //Zoom in and out with the mouse scroll wheel
        void LateUpdate()
        {
            ZoomCamera(); // Call the scroll method to handle zooming
            FollowPlayer(); // Call the method to follow the player
            RotateCamera(); // Call the method to rotate the camera around the player
        }

        //Method to scroll the camera with the mouse wheel or smartphone touch
        public void ZoomCamera()
        {
            //check if is in standalone mode or in smartphone mode
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            //float scroll = Input.GetAxis("Mouse ScrollWheel");
            //if (scroll != 0f)
            //{
            //    Camera.main.fieldOfView -= scroll * scrollSpeed;
            //    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 45f, 90f);
            //}
#endif
#if MOBILE
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                Camera.main.fieldOfView += deltaMagnitudeDiff * scrollSpeed * Time.deltaTime;
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 45f, 90f);
            }
#endif

        }

        //Method to follow the player at a fixed distance
        public void FollowPlayer()
        {
            if (player != null)
            {
                // Update the camera position to follow the player
                transform.position = player.transform.position + distanceFromPlayer;
                transform.LookAt(player.transform); // Make the camera look at the player
            }
            else
            {
                Debug.LogWarning("Player object is not assigned or found.");
            }
        }

        //Method to rotate the camera around the player, controlled by mouse movement or smartphone touch
        public void RotateCamera()
        {
            // Check if in standalone mode or smartphone mode
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR

            //if the right mouse button is pressed, rotate the camera 
            //if (Input.GetMouseButton(1))
            //{
            //    float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            //    transform.RotateAround(player.transform.position, Vector3.up, horizontal);
            //    //update the distance from the player
            //    distanceFromPlayer = transform.position - player.transform.position;
            //}
#endif

#if MOBILE
            // Check if there is a touch input
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    float horizontal = touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                    transform.RotateAround(player.transform.position, Vector3.up, horizontal);
                    //update the distance from the player
                    distanceFromPlayer = transform.position - player.transform.position;
                }
            }
#endif
        }
    }
}