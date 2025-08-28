using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MessageSystem
{
    public class ConversationView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject _content;
        [SerializeField] private GameObject _notificaton;
        [SerializeField] private GameObject _conversations;
        [SerializeField] private GameObject _conversationLog;

        [Header("References")]
        [SerializeField] private GameObject _conversation_prefab;
        [SerializeField] private GameObject _messageLeft_prefab;
        [SerializeField] private GameObject _messageRight_prefab;


        public bool IsOpen => _content.activeSelf;

        private ConversationData _conversationData_cache;

        public void AddNotification()
        {
            _notificaton.gameObject.SetActive(true);
        }

        public void Open()
        {
            _content.SetActive(true);
            _notificaton.SetActive(false);
            OpenConversations();
        }

        public void OpenConversations()
        {
            _conversationLog.SetActive(false);
            _conversations.SetActive(true);

            var content = _conversations.transform.GetChild(0).GetChild(0).GetChild(0);

            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            var data = SaveSystem.SaveHandler.GetGameData();

            foreach (var conversation in data.conversationData)
            {
                var entry = Instantiate(_conversation_prefab, content);
                entry.transform.Find("User ID").GetComponent<TextMeshProUGUI>().text = conversation.ID;

                var log = conversation.GetLog;

                entry.transform.Find("Last Message").GetComponent<TextMeshProUGUI>().text = log[log.Length - 1].message;

                entry.GetComponent<Button>().onClick.AddListener(() => { OpenConversation(conversation); });
            }

            _conversationData_cache = null;
        }

        public void OpenConversation(ConversationData data)
        {
            _conversationData_cache = data;

            _conversationLog.SetActive(true);
            _conversations.SetActive(false);

            _conversationLog.transform.GetChild(1).GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
            Canvas.ForceUpdateCanvases();

            var content = _conversationLog.transform.GetChild(1).GetChild(0).GetChild(0);

            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            var log = data.GetLog;

            foreach (var entry in log)
            {
                if (entry.senderID == "player")
                {
                    var bubble = Instantiate(_messageRight_prefab, content);
                    bubble.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.message;
                }
                else
                {
                    var bubble = Instantiate(_messageLeft_prefab, content);
                    bubble.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.message;
                }
            }
        }

        public void ReceiveMessage()
        {
            if (_conversationData_cache != null)
            {
                OpenConversation(_conversationData_cache);
            }
            else
            {
                OpenConversations();
            }
        }
    }
}