using NUnit.Framework;
using RythmGame;
using UnityEngine;



namespace GolfGame
{
    public class GameController : MonoBehaviour, IGameController
    {
        public PlayerInputActions inputActions;
        private PlayerController playerController;

        

        private void Awake()
        {
            inputActions = new PlayerInputActions();
            inputActions.GolfMiniGame.Enable();

            //Movimiento
            //inputActions.GolfMiniGame.LeftDirection.performed += ctx => 

            //Lanzamiento

        }
        private void Start()
        {
            
        }
        private void Update()
        {
            
        }

        public void HandleMessage(string message)
        {

        }
    }

}
