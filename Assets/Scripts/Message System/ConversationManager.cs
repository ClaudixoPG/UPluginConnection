using UnityEngine;

namespace MessageSystem
{
    public class ConversationManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ConversationView _view;

        private void Awake()
        {
            SaveSystem.GameData.onMessageReceive += GameData_onMessageReceive;
        }

        private void GameData_onMessageReceive(ConversationData conversation)
        {
            if (!Application.isPlaying) return;

            if (!_view.IsOpen)
            {
                _view.AddNotification();
            }
            else
            {
                _view.ReceiveMessage();
            }
        }

        public static void SendMessage(string conversationID, string senderID, string message)
        {
            var data = SaveSystem.SaveHandler.GetGameData();

            if (data.ConversationExists(conversationID))
            {
                var conversation = data.GetConversation(conversationID);
                conversation.Sendmessage(senderID, message);

                data.SetConversation(conversation);
            }
            else
            {
                var conversation = new ConversationData(conversationID, senderID, message);
                data.SetConversation(conversation);
            }
        }
    }
}
