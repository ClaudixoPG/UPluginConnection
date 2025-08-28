using MessageSystem;
using System.Collections.Generic;
using UnityEngine;
using static SaveSystem.GameData;

namespace SaveSystem
{
    [System.Serializable]
    public class GameData
    {
        public string username;

        public List<ConversationData> conversationData = new List<ConversationData>();

        public delegate void OnMessageReceive(ConversationData conversation);
        public static event OnMessageReceive onMessageReceive;


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
    }
}
