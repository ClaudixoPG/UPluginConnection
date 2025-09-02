using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace GolfGame
{
    public class HUDController : MonoBehaviour
    {
        

        [Header("UI References")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text launchText;

        [Header("Gameplay Settigs")]
        [SerializeField] private float startTime = 120f; //tiempo en segundos


        private float currentTime;
        private int launch = 0;

        private void Awake()
        {
           
           //DontDestroyOnLoad(gameObject);

            currentTime = startTime;
        }
        private void Start()
        {           
            UI();
        }

        private void Update()
        {
            UpdateUI();
        }


        public void AddLaunch()
        {
            launch++;
            UI();
        }

        private void UI()
        {
            // Mostrar en formato mm:ss
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            launchText.text = "Lanzamientos: " + launch;
        }
        private void UpdateUI()
        {
            // Reducir tiempo
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                {
                    currentTime = 0;
                    PlayerLose();
                }
                UI();
            }
        }

        private void PlayerLose()
        {
            //Implementar logica de perder
        }
    }

}
