using DialogueSystem;
using MinigameSystem;
using QuestSystem;
using SaveSystem;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PointOfInterest))]
public class POI_InteractionHandler : MonoBehaviour
{
    [System.Serializable]
    public struct InteractionData
    {
        [Header("Mandatory Settings")]
        [SerializeField] public string poi_interaction_id;
        [Tooltip("The dialogue triggered by this interaction POI")]
        [SerializeField] public DialogueModel triggeredDialogue;

        [Header("Optional Settings")]
        [Tooltip("If its in blank not need any quest to trigger")]
        [SerializeField] public string quest_id;
        [Tooltip("If its in blank only need to be in the same quest id, must need 'quest_id' not be blank")]
        [SerializeField] public string quest_objective_id;
        [Tooltip("If its in blank not trigger minigame after dialogue")]
        [SerializeField] public string minigame_name;
        [Tooltip("Can repeat this interaction")]
        [SerializeField] public bool isRepeatable;

        public string GetInteractionID => quest_objective_id;

        public readonly bool CanBeActivated
        {
            get
            {
                //Check if need quest for interaction
                if (quest_id != string.Empty)
                {
                    //Check if not need objective
                    if(quest_objective_id == string.Empty)
                    {
                        //Check if can repeat this interaction
                        if (isRepeatable) return true;

                        //Check if was interacted previusly
                        return !SaveHandler.GetGameData().completedQuestObjectiveIDs.Contains(poi_interaction_id);
                    }

                    //Check if the player is in the matching quest
                    if (quest_id != QuestSystemManager.Singleton.GetCurrentQuestID()) return false;

                    var currentObjective = QuestSystemManager.Singleton.GetCurrentQuestObjective();

                    if (currentObjective != null)
                    {
                        //Match current quest objective
                        if (currentObjective.quest_objective_id == quest_objective_id)
                        {
                            //Check if can repeat this interaction
                            if (isRepeatable) return true;

                            //Check if was interacted previusly
                            return !SaveHandler.GetGameData().completedQuestObjectiveIDs.Contains(poi_interaction_id);
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return false;
                }
                else
                {
                    //Check if can repeat this interaction
                    if (isRepeatable) return true;

                    //Check if was interacted previusly
                    return !SaveHandler.GetGameData().completedQuestObjectiveIDs.Contains(poi_interaction_id);
                }
            }
        }
    }

    [SerializeField] private InteractionData[] aviableInteractions;

    private PointOfInterest _myPOI;

    private static InteractionData? _currentInteracting;


    void Awake()
    {
        _myPOI = GetComponent<PointOfInterest>();
    }

    private void Start()
    {
        CustomInputHandler.onReceiveKey += ListenKey;
    }

    private void ListenEndDialogue()
    {
        if (_currentInteracting == null) return;

        if (_currentInteracting.Value.minigame_name != string.Empty)
        {
            MinigamesManager.PlayMinigame(_currentInteracting.Value.minigame_name, _currentInteracting.Value.quest_id, _currentInteracting.Value.quest_objective_id);
        }
        else
        {
            QuestSystem.QuestSystemManager.Singleton.CheckQuest(_currentInteracting.Value.quest_id, _currentInteracting.Value.quest_objective_id);
        }
    }

    private void ListenKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.KeypadEnter:

                if (_myPOI.IsDetected)
                {
                    foreach (var interaction in aviableInteractions)
                    {
                        if (interaction.CanBeActivated)
                        {
                            BeginInteraction(interaction);
                            return;
                        }
                    }
                }

                break;
        }
    }

    public bool CanBeInteractWithAny()
    {
        foreach (var interaction in aviableInteractions)
        {
            if (interaction.CanBeActivated)
            {
                return true;
            }
        }

        return false;
    }

    public void BeginInteraction(InteractionData interactionData)
    {
        _currentInteracting = interactionData;

        if (_currentInteracting.Value.triggeredDialogue != null)
        {
            if(_currentInteracting.Value.quest_objective_id != string.Empty)
                SaveHandler.GetGameData().MarkQuestObjectiveCompleted(_currentInteracting.Value.quest_objective_id);

            DialogueManager.PlayDialogue(_currentInteracting.Value.triggeredDialogue, ListenEndDialogue);
        }
    }
}
