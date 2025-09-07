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
            QuestSystemManager.onQuestStatusUpdate += Listener_QuestUpdate;
            QuestSystemManager.onQuestCompleted += Listener_QuestCompleted;
        }

        private void Listener_QuestUpdate(QuestData questData, int currentPOI)
        {
            var objective = questData.POI_Ids[currentPOI];
            DisplayBanner(objective.title, objective.description);
        }

        private void Listener_QuestCompleted(QuestData questData)
        {
            DisplayBanner(questData.questName, "Completado");
        }

        public void DisplayBanner(string title, string subtext)
        {
            _title_text.text = title;
            _subtext_text.text = subtext;

            _banner_animator.SetTrigger("Play");
        }
    }
}
