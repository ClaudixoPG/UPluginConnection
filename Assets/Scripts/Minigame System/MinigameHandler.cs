using DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MinigameSystem
{
    public abstract class MinigameHandler : MonoBehaviour
    {
        private string _questID, _poiInteractionID;

        private bool _wasInit;

        private void Awake()
        {
            //Execute the scene without other systems
            if (UnityEngine.SceneManagement.SceneManager.sceneCount == 1)
            {
                Init(string.Empty, string.Empty);
            }
        }

        public void Init(string questID, string poiInteractionID)
        {
            _questID = questID;
            _poiInteractionID = poiInteractionID;

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
        public void CompleteGame(string minigameName, string log)
        {
            MinigamesManager.CloseMinigame(minigameName, log);
        }
    }
}
