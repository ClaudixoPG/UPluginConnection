using TMPro;
using UnityEngine;

namespace QuestSystem.Utils
{
    public class QuestBanner : MonoBehaviour
    {
        [SerializeField] private Animator _banner_animator;
        [SerializeField] private TextMeshProUGUI _title_text;
        [SerializeField] private TextMeshProUGUI _subtext_text;

        private void Awake()
        {
            QuestSystemManager.onQuestStatusUpdate += AwaitForNewQuest;
        }

        private void AwaitForNewQuest(QuestData questData, int currentPOI)
        {
            var poi = POIManager.Instance.GetPOI(questData.POI_Ids[currentPOI]);
            DisplayBanner(questData.questName, poi.ClueMessage);
        }

        public void DisplayBanner(string title, string subtext)
        {
            _title_text.text = title;
            _subtext_text.text = subtext;

            _banner_animator.SetTrigger("Play");
        }
    }
}
