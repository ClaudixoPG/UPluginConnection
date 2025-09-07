using DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MinigameSystem
{
    public abstract class MinigameHandler : MonoBehaviour
    {
        private static GameObject __minigameParent;

        private string _questID, _poiInteractionID;

        private bool _wasInit;

        public delegate void OnCompleteGame(string questID, string _poiInteractionID);
        public static event OnCompleteGame onCompleteGame;

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

        public static void PlayMinigame(string minigamePrefab, string questID, string poiInteractionID)
        {
            if (__minigameParent == null)
            {
                __minigameParent = new GameObject("Minigame");

                MinigameHandler miniGamePrefab = Resources.Load<MinigameHandler>($"Minigames/{minigamePrefab}");

                var game = Instantiate(miniGamePrefab, __minigameParent.transform);
                game.Init(questID, poiInteractionID);
            }
            else
            {
                Debug.LogError("You are trying to execute a game but a game are running");
            }
        }

        protected abstract void OnStartGame();
        protected abstract void UpdateGame();
        public void CompleteGame()
        {
            onCompleteGame?.Invoke(_questID, _poiInteractionID);
            Destroy(__minigameParent);
        }
    }
}
