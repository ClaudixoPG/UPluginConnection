using DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MinigameSystem
{
    public class MinigamesManager : MonoBehaviour
    {
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private EventSystem _worldEventSystem;

        public delegate void OnCompleteGame(string questID, string _poiInteractionID);
        public static event OnCompleteGame onCompleteGame;

        private static string _currentQuestID;
        private static string _currentPointInteractionID;
        private static Scene _minigameScene;

        private static MinigamesManager _singleton;

        public static MinigamesManager Singleton
        {
            get
            {
                if(_singleton == null)
                    _singleton = FindFirstObjectByType<MinigamesManager>();

                return _singleton;
            }
        }

        public static void CloseMinigame()
        {
            onCompleteGame?.Invoke(_currentQuestID, _currentPointInteractionID);

            Singleton._worldCamera.gameObject.SetActive(true);
            Singleton._worldEventSystem.gameObject.SetActive(true);

            _currentQuestID = string.Empty;
            _currentPointInteractionID = string.Empty;

            if (UnityEngine.SceneManagement.SceneManager.sceneCount > 1)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_minigameScene);
                _minigameScene = default;
            }
        }

        public static void PlayMinigame(string minigameScene, string questID, string poiInteractionID)
        {
            Singleton._worldCamera.gameObject.SetActive(false);
            Singleton._worldEventSystem.gameObject.SetActive(false);

            _currentQuestID = questID;
            _currentPointInteractionID = poiInteractionID;
            Singleton.StartCoroutine(Singleton.LoadGame(minigameScene, questID, poiInteractionID));
        }

        private IEnumerator LoadGame(string minigameSceneName, string questID, string poiInteractionID)
        {
            // Cargar la escena de forma aditiva
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Buscar la escena recién cargada
            var loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(minigameSceneName);

            if (loadedScene.IsValid())
            {
                // Buscar todos los root objects de esa escena
                var rootObjects = loadedScene.GetRootGameObjects();

                _minigameScene = loadedScene;

                // Localizar el MinigameHandler
                foreach (var root in rootObjects)
                {
                    var handler = root.GetComponentInChildren<MinigameHandler>(true);
                    if (handler != null)
                    {
                        handler.Init(questID, poiInteractionID);
                        Debug.Log($"MinigameHandler encontrado en escena {minigameSceneName} y inicializado.");
                        yield break;
                    }
                }

                Debug.LogError($"No se encontró un MinigameHandler en la escena {minigameSceneName}");
            }
            else
            {
                Debug.LogError($"La escena {minigameSceneName} no es válida o no se pudo cargar.");
            }
        }
    }
}
