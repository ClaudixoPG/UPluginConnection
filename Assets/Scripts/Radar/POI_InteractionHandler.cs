using DialogueSystem;
using QuestSystem;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PointOfInterest))]
public class POI_InteractionHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string minigame_name;

    [Header("References")]
    [SerializeField] private DialogueModel _triggeredDialogue;

    private PointOfInterest _myPOI;

    void Awake()
    {
        _myPOI = GetComponent<PointOfInterest>();
    }

    private void Start()
    {
        CustomInputHandler.onReceiveKey += ListenKey;
        DialogueSceneHandler.onEndDialogue += ListenEndDialogue;
    }

    private void ListenEndDialogue(string dialogueID)
    {
        if (dialogueID == _triggeredDialogue.dialogueID)
        {
            Debug.Log("Open Minigame: " + minigame_name);
            //QuestSystemManager.Singleton.TryCompletePOI(_myPOI.ID);
        }
    }

    private void ListenKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.KeypadEnter:

                if (_myPOI.IsDetected)
                {
                    BeginInteraction();
                }

                break;
        }
    }

    public void BeginInteraction()
    {
        if(_triggeredDialogue != null)
        {
            DialogueManager.PlayDialogue(_triggeredDialogue);
        }
    }
}
