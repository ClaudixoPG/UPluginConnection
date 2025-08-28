using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DialogueSystem
{
    /// <summary>
    /// Manages the dialogue system lifecycle, including initialization, 
    /// scene loading, and triggering dialogue playback.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private DialogueModel _testDialogue;

        /// <summary>
        /// Singleton instance of the DialogueManager.
        /// </summary>
        private static DialogueManager _singleton;

        /// <summary>
        /// Indicates whether a dialogue is currently being played.
        /// </summary>
        private bool _isPlaying;

        /// <summary>
        /// Provides access to the singleton instance of the DialogueManager.
        /// </summary>
        private static DialogueManager Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = FindFirstObjectByType<DialogueManager>();

                return _singleton;
            }
        }

        [ContextMenu("Test Dialogue")]
        private void TestDialogue()
        {
            if (_testDialogue != null)
                PlayDialogue(_testDialogue);
        }

        /// <summary>
        /// Starts a dialogue sequence if no other dialogue is currently playing.
        /// </summary>
        /// <param name="dialogueModel">The dialogue data to play.</param>
        public static void PlayDialogue(DialogueModel dialogueModel)
        {
            if (!Singleton._isPlaying)
            {
                Singleton.InitDialogue(dialogueModel);
            }
        }

        /// <summary>
        /// Initializes the dialogue sequence and begins loading the dialogue scene.
        /// </summary>
        /// <param name="dialogueModel">The dialogue data to initialize.</param>
        private void InitDialogue(DialogueModel dialogueModel)
        {
            Singleton._isPlaying = true;

            StartCoroutine(LoadDialogueScene(dialogueModel));
        }

        /// <summary>
        /// Loads the dialogue scene asynchronously and plays the dialogue once loaded.
        /// </summary>
        /// <param name="dialogueModel">The dialogue data to be used in the scene.</param>
        /// <returns>Coroutine enumerator for asynchronous scene loading.</returns>
        private IEnumerator LoadDialogueScene(DialogueModel dialogueModel)
        {
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Dialogue Scene", LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Scene dialogueScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Dialogue Scene");
            if (dialogueScene.IsValid() && dialogueScene.isLoaded)
            {
                DialogueSceneHandler handler = null;
                foreach (GameObject root in dialogueScene.GetRootGameObjects())
                {
                    handler = root.GetComponentInChildren<DialogueSceneHandler>();
                    if (handler != null)
                        break;
                }
                if (handler != null)
                {
                    handler.Play(dialogueModel);
                }
            }
        }
    }
}