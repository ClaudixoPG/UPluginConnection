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

        public delegate void OnCompleteGame(string questID, string _poiInteractionID, string log);
        public static event OnCompleteGame onCompleteGame;

        private static string _currentQuestID;
        private static string _currentQuestObjective_ID;
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

        public static void CloseMinigame(string minigameName, float percentage)
        {
            SaveSystem.SaveHandler.GetGameData().AddLog("[MINIGAME]", minigameName, percentage);
            onCompleteGame?.Invoke(_currentQuestID, _currentQuestObjective_ID, minigameName);

            Singleton._worldCamera.gameObject.SetActive(true);
            Singleton._worldEventSystem.enabled = true;

            _currentQuestID = string.Empty;
            _currentQuestObjective_ID = string.Empty;

            if (UnityEngine.SceneManagement.SceneManager.sceneCount > 1)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_minigameScene);
                _minigameScene = default;
            }
        }

        public static void PlayMinigame(string minigameScene, string questID, string objectiveID)
        {
            Singleton._worldCamera.gameObject.SetActive(false);
            Singleton._worldEventSystem.enabled = false;

            _currentQuestID = questID;
            _currentQuestObjective_ID = objectiveID;
            Singleton.StartCoroutine(Singleton.LoadGame(minigameScene));
        }

        private IEnumerator LoadGame(string minigameSceneName)
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
                        handler.Init();
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
