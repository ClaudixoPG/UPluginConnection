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
    }
}
