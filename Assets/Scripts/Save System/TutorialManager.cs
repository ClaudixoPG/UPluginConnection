using DialogueSystem;
using SaveSystem;
using SaveSystem.Extras;
using UnityEngine;

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

    [Header("Dialogues References")]
    /// <summary>
    /// Reference to the dialogue that should play at the beginning of the game
    /// if no saved data is found (the game's prologue).
    /// </summary>
    [SerializeField]
    private DialogueModel _prologueDialogue;
    [SerializeField]
    private DialogueModel _welcomeDialogue;


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

        RegisterWindow.onCompleteRegistration += DisplayWelcome;

        DialogueSceneHandler.onEndDialogue += DialogueEndListener;
    }

    private void DialogueEndListener(string dialogueID)
    {
        if (_welcomeDialogue.dialogueID == dialogueID)
        {
            MessageSystem.ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "Hola, espero no haberme equivocado de numero jaja");
            MessageSystem.ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "Por cierto, necesitaras tu credencial");
            // Iniciar mision 'Find your Credential'

            MessageSystem.ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "Puedes encontrarla utilizando el sistema de radar que funciona asi:");
            MessageSystem.ConversationManager.SendMessage(_senderMessage_name, _senderMessage_name, "[Explicacion]");
            return;
        }
    }

    private void DisplayWelcome()
    {
        DialogueManager.PlayDialogue(_welcomeDialogue);
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
            DialogueManager.PlayDialogue(_prologueDialogue);
            _register_window.SetActive(true);
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
}