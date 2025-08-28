using UnityEngine;
using UnityEditor;

namespace MessageSystem.EditorTools
{
    /// <summary>
    /// Custom EditorWindow for sending messages to a ConversationData object.
    /// Provides fields for entering a conversation ID, sender ID, and a message, along with a send button.
    /// </summary>
    public class ConversationEditorWindow : EditorWindow
    {
        private string _conversationID = string.Empty;
        private string _senderID = string.Empty;
        private string _message = string.Empty;

        /// <summary>
        /// Adds a new option in the Unity Editor toolbar to open this window.
        /// </summary>
        [MenuItem("Tools/Message System/Conversation Editor")]
        public static void ShowWindow()
        {
            GetWindow<ConversationEditorWindow>("Conversation Editor");
        }

        /// <summary>
        /// Draws the UI for the editor window.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Conversation Editor", EditorStyles.boldLabel);

            _conversationID = EditorGUILayout.TextField("Conversation ID", _conversationID);
            _senderID = EditorGUILayout.TextField("Sender ID", _senderID);
            _message = EditorGUILayout.TextField("Message", _message);

            GUILayout.Space(10);

            if (GUILayout.Button("Send Message"))
            {
                SendMessageToConversation(_conversationID, _senderID, _message);
            }
        }

        /// <summary>
        /// Handles sending messages to a conversation.
        /// Implementation can be expanded to create the conversation if it does not exist.
        /// </summary>
        /// <param name="conversationID">The ID of the conversation.</param>
        /// <param name="senderID">The ID of the message sender.</param>
        /// <param name="message">The message to send.</param>
        private void SendMessageToConversation(string conversationID, string senderID, string message)
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

