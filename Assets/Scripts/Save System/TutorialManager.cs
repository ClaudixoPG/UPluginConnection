using DialogueSystem;
using MessageSystem;
using QuestSystem;
using SaveSystem;
using SaveSystem.Extras;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the player's saved game data at the start of the game.
/// This component should be placed in the scene to check whether
/// a save file exists. 
/// - If no data exists, it starts the prologue dialogue 
///   and opens the registration window for a new player.
/// - If data exists, it assumes the player is already registered 
///   and skips the registration flow.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string _senderMessage_name = "Ramon";

    [Header("Quest References")]
    [SerializeField]
    private QuestData _findYourCredentials_questline;


    /// <summary>
    /// UI window that allows new players to register. 
    /// It is hidden by default and only displayed if no saved data is found.
    /// </summary>
    [SerializeField]
    private GameObject _register_window;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Ensures that the registration window is hidden at the start.
    /// </summary>
    private void Awake()
    {
        _register_window.SetActive(false);

        PrologueSceneManager.onPrologueSceneEnds += Listener_DisplayForm;
        RegisterWindow.onCompleteRegistration += Listener_OnRegisterComplete;

        DialogueSceneHandler.onEndDialogue += Listener_DialogueEnd;
    }

    private void Listener_OnRegisterComplete()
    {
        _register_window.SetActive(false);

        ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "Wena, ya llegaste?");
        ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "Estoy aca en la U");

        ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "[carrera_videojuegos]");

        ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "Tenis que venir pa ca a buscar tu credencial");
        ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "Ah, y prende el radar cuando caminis, asi te marca altiro donde queda la cuestion");

        QuestSystemManager.Singleton.AssignQuest(_findYourCredentials_questline);
    }

    private void Listener_DialogueEnd(string dialogueID)
    {
        
    }

    private void Listener_DisplayForm()
    {
        _register_window.SetActive(true);
    }

    /// <summary>
    /// Called before the first frame update.
    /// Checks whether a save file exists:
    /// - If not, it starts the prologue dialogue and enables the registration window.
    /// - If yes, it does nothing and allows the player to continue with existing data.
    /// </summary>
    private void Start()
    {
        if (!SaveHandler.GameDataExists())
        {
            StartCoroutine(LoadPrologueScene());
        }
    }

    /// <summary>
    /// Debug/utility method to remove all stored game data.
    /// Can be executed manually from the component's context menu in the Inspector.
    /// </summary>
    [ContextMenu("Remove All Stored Data")]
    private void RemoveAllStoredData()
    {
        SaveHandler.Delete();
    }

    /// <summary>
    /// Loads the prologue scene asynchronously
    /// </summary>
    /// <param name="dialogueModel">The dialogue data to be used in the scene.</param>
    /// <returns>Coroutine enumerator for asynchronous scene loading.</returns>
    private IEnumerator LoadPrologueScene()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Prologue", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}