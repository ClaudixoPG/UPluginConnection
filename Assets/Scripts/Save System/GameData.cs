using MessageSystem;
using QuestSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static SaveSystem.GameData;

namespace SaveSystem
{
    [System.Serializable]
    public struct StadisticsLog
    {
        public string stadisticName;
        public string log;
        public float percentage;

        public StadisticsLog(string stadisticName, string log, float percentage)
        {
            this.stadisticName = stadisticName;
            this.log = log;
            this.percentage = percentage;
        }
    }

    [System.Serializable]
    public class GameData
    {
        public string username;
        public string uniqueID;
        public int age;

        public List<QuestlineCacheData> questlineCacheDatas = new List<QuestlineCacheData> ();
        public List<ConversationData> conversationData = new List<ConversationData>();
        public List<string> completedQuestObjectiveIDs = new List<string>();

        public List<StadisticsLog> stadisticsLog = new List<StadisticsLog>();

        public string UniqueID
        {
            get
            {
                if (uniqueID == null || uniqueID == string.Empty)
                {
                    uniqueID = GenerateCode();
                    SaveHandler.Save();
                }

                return uniqueID;
            }
        }

        public GameData(string username, int age)
        {
            this.username = username;
            this.age = age;
            uniqueID = GenerateCode();
        }

        public GameData()
        {
            username = string.Empty;
            age = -1;
        }

        public delegate void OnMessageReceive(ConversationData conversation);
        public static event OnMessageReceive onMessageReceive;

        public void AddLog(string stadisticName, string log, float percentage)
        {
            stadisticsLog.Add(new StadisticsLog(stadisticName, log, percentage));
            SaveHandler.Save();
        }

        public QuestlineCacheData GetQuestCache(string questID)
        {
            foreach (var quest in questlineCacheDatas)
            {
                if (quest.questlineID == questID)
                    return quest;
            }

            return null;
        }

        public QuestlineCacheData GetLastQuestLine()
        {
            if (questlineCacheDatas.Count > 0)
                return questlineCacheDatas.Last();

            return null;
        }

        public int GetStoredIndexQuestline(string questlineID)
        {
            foreach (var questline in questlineCacheDatas)
            {
                if(questline.questlineID == questlineID)
                {
                    return questline.storedIndex;
                }
            }

            return -1;
        }

        public bool StoredQuestlineExists(string questlineID)
        {
            return questlineCacheDatas.Any(x => x.questlineID == questlineID);
        }

        public void SaveQuestline(string questlineID, int currentIndex)
        {
            for (int i = 0; i < questlineCacheDatas.Count; i++)
            {
                if (questlineCacheDatas[i].questlineID == questlineID)
                {
                    questlineCacheDatas[i].SetIndex(currentIndex);
                    return;
                }
            }

            var cache = new QuestlineCacheData(questlineID, currentIndex);
            questlineCacheDatas.Add(cache);

            SaveHandler.Save();
        }

        public void MarkQuestObjectiveCompleted(string questObjectiveID)
        {
            if (completedQuestObjectiveIDs == null) completedQuestObjectiveIDs = new List<string>();

            if (!completedQuestObjectiveIDs.Contains(questObjectiveID))
            {
                completedQuestObjectiveIDs.Add(questObjectiveID);
                SaveHandler.Save();
            }
        }

        /// <summary>
        /// Adds or replaces a conversation in the list.
        /// If a conversation with the same ID already exists, it will be replaced.
        /// Otherwise, the new conversation will be added.
        /// </summary>
        /// <param name="conversationData">The conversation data to add or replace.</param>
        public void SetConversation(ConversationData conversationData)
        {
            if (conversationData == null)
                return;

            if (ConversationExists(conversationData.ID))
            {
                int index = this.conversationData.FindIndex(c => c.ID == conversationData.ID);
                this.conversationData[index] = conversationData;
                onMessageReceive?.Invoke(this.conversationData[index]);
            }
            else
            {
                this.conversationData.Add(conversationData);
                onMessageReceive?.Invoke(conversationData);
            }

            SaveHandler.Save();
        }

        /// <summary>
        /// Checks if a conversation with the given ID exists.
        /// </summary>
        /// <param name="id">The ID of the conversation to search for.</param>
        /// <returns>True if the conversation exists, false otherwise.</returns>
        public bool ConversationExists(string id)
        {
            return conversationData.Exists(c => c.ID == id);
        }

        /// <summary>
        /// Retrieves a conversation by its unique ID.
        /// Returns null if no conversation with the given ID exists.
        /// </summary>
        /// <param name="id">The unique identifier of the conversation.</param>
        /// <returns>The conversation data if found; otherwise, null.</returns>
        public ConversationData GetConversation(string id)
        {
            return conversationData.Find(c => c.ID == id);
        }

        public static string GenerateCode(int length = 10)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = Random.Range(0, chars.Length);
                result.Append(chars[index]);
            }

            return result.ToString();
        }
    }
}
