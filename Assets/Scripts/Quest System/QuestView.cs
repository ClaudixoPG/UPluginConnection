using TMPro;
using UnityEngine;

namespace QuestSystem
{
    public class QuestView : MonoBehaviour
    {
        private static QuestView _singleton;

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _questTitle_text;
        [SerializeField] private TextMeshProUGUI _currentPOITitle_text;

        public static QuestView Singleton
        {
            get
            {
                if(_singleton == null)
                    _singleton = FindFirstObjectByType<QuestView>(FindObjectsInactive.Include);

                return _singleton;
            }
        }

        void Awake()
        {
            Hide();   
        }

        public void Paint(QuestData currentQuestData, int currentPoiIndex)
        {
            gameObject.SetActive(true);

            _questTitle_text.text = currentQuestData.questName;
            _currentPOITitle_text.text = currentQuestData.POI_Ids[currentPoiIndex];
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}