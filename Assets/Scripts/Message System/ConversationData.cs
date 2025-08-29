using UnityEngine;
using System.Collections.Generic;

namespace MessageSystem
{
    [System.Serializable]
    public class ConversationData
    {
        [System.Serializable]
        public class MessageEntry
        {
            public string senderID;
            public string message;

            public MessageEntry(string senderID, string message)
            {
                this.senderID = senderID;
                this.message = message;
            }
        }

        [SerializeField] private string _conversationID;

        [SerializeField] private List<MessageEntry> _log = new List<MessageEntry>();

        public string ID => _conversationID;

        public MessageEntry[] GetLog => _log.ToArray();

        public ConversationData(string conversationID)
        {
            _conversationID = conversationID;
            _log = new List<MessageEntry>();
        }

        public ConversationData(string conversationID, string senderID, string message)
        {
            _conversationID = conversationID;
            _log = new List<MessageEntry>();

            Sendmessage(senderID, message);
        }

        public void Sendmessage(string senderID, string message)
        {
            _log.Add(new MessageEntry(senderID, message));
        }
    }
}
