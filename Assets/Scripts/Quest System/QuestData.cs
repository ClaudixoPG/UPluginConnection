using System;
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

        public string quest_objective_id;
        public string targetPOI_id;
        public string title;
        public string description;

        /// <summary>
        /// The number of time in minutes to complete a quest before turn it a incomplete quest. If '0' means not have timer.
        /// </summary>
        public long questCompletionTime;

        public List<MessageReward> messagesOnComplete = new List<MessageReward>();
        public DialogueSystem.DialogueModel dialogueOnComplete;

        public List<MessageReward> messagesOnFail = new List<MessageReward>();
    }

    [System.Serializable]
    public class QuestlineCacheData
    {
        public string questlineID;
        public int storedIndex;
        public long acceptTime;

        public QuestlineCacheData()
        {
        }

        public QuestlineCacheData(string questlineID, int storedIndex)
        {
            this.questlineID = questlineID;
            this.storedIndex = storedIndex;
            acceptTime = DateTime.Now.ToBinary();
        }

        public void SetIndex(int storedIndex)
        {
            this.storedIndex = storedIndex;
            acceptTime = DateTime.Now.ToBinary();
        }
    }
}