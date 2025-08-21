using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Manages the current quest, tracking its progress and informing the player with relevant updates.
    /// </summary>
    public class QuestSystemManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private QuestData _currentQuest;

        [Header("Components")]
        [SerializeField] private GameObject _questMark; //The GameObject used as a visual marker displayed above the target POI.

        /// <summary>
        /// Index of the next Point of Interest (POI) the player must visit in the current quest.
        /// </summary>
        private int _questIndex;

        private void Awake()
        {
            _questMark.gameObject.SetActive(false);
            AssignQuest(_currentQuest);
        }

        /// <summary>
        /// Assigns a new quest to the player. 
        /// If the player was already on a quest, it will be overwritten to start the new questline.
        /// </summary>
        public void AssignQuest(QuestData questData)
        {
            _currentQuest = questData;

            if (_currentQuest != null)
            {
                _questIndex = RememberIndex();
            }

            UpdateQuest();
        }

        /// <summary>
        /// Updates the quest UI with the current quest information. 
        /// If no quest is active, the UI is updated to reflect the absence of an active quest.
        /// </summary>
        private void UpdateQuest()
        {
            if (_currentQuest == null)
            {
                QuestView.Singleton.Hide();
                return;
            }

            _questMark.SetActive(true);

            var poi = POIManager.Instance.GetPOI(_currentQuest.POI_Ids[_questIndex]);

            _questMark.transform.position = poi.transform.position + Vector3.up * 30;

            QuestView.Singleton.Paint(_currentQuest, _questIndex);
        }

        /// <summary>
        /// Remembers the last visited POI index in the current quest line.
        /// Not yet implemented; currently always returns the first POI (index 0).
        /// </summary>
        /// <returns>The index of the last visited POI (currently always 0).</returns>
        private int RememberIndex()
        {
            return 0;
        }
    }
}
