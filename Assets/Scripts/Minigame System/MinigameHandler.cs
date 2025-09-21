using DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MinigameSystem
{
    public abstract class MinigameHandler : MonoBehaviour
    {
        private bool _wasInit;

        private void Awake()
        {
            //Execute the scene without other systems
            if (UnityEngine.SceneManagement.SceneManager.sceneCount == 1)
            {
                Init();
            }
        }

        public void Init()
        {
            OnStartGame();

            _wasInit = true;
        }

        private void Update()
        {
            if (_wasInit)
            {
                UpdateGame();
            }
        }

        protected abstract void OnStartGame();
        protected abstract void UpdateGame();
        public void CompleteGame(string minigameName, float percentage)
        {
            MinigamesManager.CloseMinigame(minigameName, percentage);
        }
    }
}
