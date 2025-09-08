using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Defines the core data of a quest, including its identifier, name, description, 
    /// and the list of associated points of interest (POIs) the player must visit.
    /// </summary>
    [CreateAssetMenu]
    public class QuestData : ScriptableObject
    {
        public string questID;
        public string questName;
        public string questDescription;

        public QuestObjectiveData[] POI_Ids;
    }

    [System.Serializable]
    public class QuestObjectiveData
    {
        [System.Serializable]
        public class MessageReward
        {
            public string senderID;
            public string message;
        }

        public string targetPOI_id;
        public string interactionPOI_id;
        public string title;
        public string description;
        public List<MessageReward> messagesOnComplete = new List<MessageReward>();
        public DialogueSystem.DialogueModel dialogueOnComplete;

    }

    [System.Serializable]
    public class QuestlineCacheData
    {
        public string questlineID;
        public int storedIndex;

        public QuestlineCacheData()
        {
        }

        public QuestlineCacheData(string questlineID, int storedIndex)
        {
            this.questlineID = questlineID;
            this.storedIndex = storedIndex;
        }
    }
}