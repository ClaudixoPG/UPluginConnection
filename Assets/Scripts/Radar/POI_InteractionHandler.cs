using DialogueSystem;
using MinigameSystem;
using QuestSystem;
using SaveSystem;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PointOfInterest))]
public class POI_InteractionHandler : MonoBehaviour
{
    [Header("Mandatory Settings")]
    [SerializeField] private string poi_interaction_id;
    [Tooltip("The dialogue triggered by this interaction POI")]
    [SerializeField] private DialogueModel _triggeredDialogue;

    [Header("Optional Settings")]
    [Tooltip("If its in blank not trigger minigame after dialogue")]
    [SerializeField] private string minigame_name;
    [Tooltip("If its in blank is not required to be in a quest to trigger")]
    [SerializeField] private string requiredQuestID;
    [Tooltip("Can repeat this interaction")]
    [SerializeField] private bool isRepeatable;
    [Tooltip("The higher the number trigger first")]
    [SerializeField] private int priority;

    private PointOfInterest _myPOI;

    public int Priority => priority;

    public string GetInteractionID => poi_interaction_id;

    public bool QuestMeetingConditions
    {
        get
        {
            if (requiredQuestID == string.Empty) return true;

            //Match current Quest
            if(requiredQuestID == QuestSystemManager.Singleton.GetCurrentQuestID())
            {
                var currentObjective = QuestSystemManager.Singleton.GetCurrentQuestObjective();

                if (currentObjective == null) return true;

                //Match current quest objective
                if (currentObjective.interactionPOI_id == poi_interaction_id)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public bool IsRepeatable => isRepeatable;

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
        if (_triggeredDialogue == null) return;
        
        if (dialogueID == _triggeredDialogue.dialogueID)
        {
            if (minigame_name != string.Empty)
            {
                MinigamesManager.PlayMinigame(minigame_name, requiredQuestID, poi_interaction_id);
            }
        }
    }

    private void ListenKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.KeypadEnter:

                if (_myPOI.IsDetected)
                {
                    if (_myPOI.CanInteract(out var poiInteraction))
                    {
                        if (poiInteraction != null && poiInteraction.poi_interaction_id == poi_interaction_id)
                        {
                            BeginInteraction();
                        }
                    }
                }

                break;
        }
    }

    public void BeginInteraction()
    {
        if(_triggeredDialogue != null)
        {
            SaveHandler.GetGameData().MarkPOIasVisited(poi_interaction_id);
            DialogueManager.PlayDialogue(_triggeredDialogue);
        }
    }
}
