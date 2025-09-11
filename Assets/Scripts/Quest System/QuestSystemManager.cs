using DialogueSystem;
using MessageSystem;
using MinigameSystem;
using NUnit.Framework.Interfaces;
using SaveSystem;
using System;
using System.Collections;
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

        private static QuestSystemManager _singleton;

        public delegate void OnStartNewQuestline(QuestData questlineData, int currentPOI);
        public delegate void OnQuestCompleted(QuestData questlineData);
        public delegate void OnQuestFail(QuestData questlineData, int currentPOI);
        public static OnStartNewQuestline onQuestStatusUpdate;
        public static OnQuestCompleted onQuestCompleted;
        public static OnQuestFail onQuestFail;

        public static QuestSystemManager Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = FindFirstObjectByType<QuestSystemManager>();

                return _singleton;
            }
        }    

        /// <summary>
        /// Index of the next Point of Interest (POI) the player must visit in the current quest.
        /// </summary>
        private int _questIndex;

        private void Awake()
        {
            _questMark.gameObject.SetActive(false);

            if (_currentQuest != null)
            {
                AssignQuest(_currentQuest);
            }

            GameData data = SaveHandler.GetGameData();
            var lastQuest = data.GetLastQuestLine();
            if (lastQuest != null)
            {
                var allQuests = Resources.LoadAll<QuestData>("Quests");

                foreach (var quest in allQuests)
                {
                    if(quest.questID == lastQuest.questlineID)
                    {
                        _currentQuest = quest;
                        _questIndex = lastQuest.storedIndex;
                        SaveHandler.GetGameData().activatedPOIs.Remove(_currentQuest.POI_Ids[_questIndex].interactionPOI_id); //Se remueve este id para garantizar que sea interactable
                        QuestView.Singleton.Paint(_currentQuest, _questIndex);
                        onQuestStatusUpdate?.Invoke(quest, _questIndex);
                        break;
                    }
                }
            }

            MinigamesManager.onCompleteGame += Listener_MinigameComplete;
        }

        private void Update()
        {
            if (_currentQuest != null)
            {
                var objective = GetCurrentQuestObjective();
                if (objective != null && objective.questCompletionTime > 0)
                {
                    var questCache = SaveHandler.GetGameData().GetQuestCache(_currentQuest.questID);

                    // Tiempo de aceptación
                    DateTime acceptTime = new DateTime(questCache.acceptTime);

                    // Duración (en minutos)
                    long durationMinutes = objective.questCompletionTime;

                    // Fecha en que debe expirar
                    DateTime expireTime = acceptTime.AddMinutes(durationMinutes);

                    TimeSpan remaining = expireTime - DateTime.Now;

                    string timerInFormat;

                    // If still valid, format as countdown
                    if (remaining.TotalSeconds > 0)
                    {
                        timerInFormat = string.Format("{0:D2}:{1:D2}", remaining.Minutes, remaining.Seconds);
                    }
                    else
                    {
                        // Already expired
                        timerInFormat = "00:00";
                    }

                    var title = objective.title;
                    var subtitle = objective.description + "\n" + timerInFormat;

                    QuestView.Singleton.Paint(title, subtitle);

                    if (DateTime.Now > expireTime)
                    {
                        // Quest Expired
                        FailQuestObjective();
                    }
                }
            }
        }

        private void Listener_MinigameComplete(string questID, string poiInteractionID)
        {
            if (_currentQuest != null)
            {
                if (_currentQuest.questID == questID && GetCurrentQuestObjective() != null && GetCurrentQuestObjective().interactionPOI_id == poiInteractionID)
                {
                    if (_currentQuest.POI_Ids[_questIndex].dialogueOnComplete != null)
                    {
                        DialogueManager.PlayDialogue(_currentQuest.POI_Ids[_questIndex].dialogueOnComplete, CompleteQuestObjective);
                    }
                    else
                    {
                        CompleteQuestObjective();
                    }
                }
            }
        }

        private void FailQuestObjective()
        {
            foreach (var message in _currentQuest.POI_Ids[_questIndex].messagesOnFail)
            {
                ConversationManager.SendMessage(message.senderID, message.senderID, message.message.Replace("%username%", SaveHandler.GetGameData().username));
            }

            var failedIndex = _questIndex;

            _questIndex++;

            SaveHandler.GetGameData().SaveQuestline(_currentQuest.questID, _questIndex);
            SaveHandler.Save();
            QuestView.Singleton.Hide();
            onQuestFail?.Invoke(_currentQuest, failedIndex);

            StartCoroutine(WaitForNewQuest());
        }

        private IEnumerator WaitForNewQuest()
        {
            yield return new WaitForSeconds(3);

            if (_questIndex < _currentQuest.POI_Ids.Length)
            {
                QuestView.Singleton.Paint(_currentQuest, _questIndex);
                onQuestStatusUpdate?.Invoke(_currentQuest, _questIndex);
                UpdateQuest();
            }
            else
            {
                onQuestCompleted?.Invoke(_currentQuest);
                QuestView.Singleton.Hide();
                _currentQuest = null;
            }
        }

        private void CompleteQuestObjective()
        {
            foreach (var reward in _currentQuest.POI_Ids[_questIndex].messagesOnComplete)
            {
                ConversationManager.SendMessage(reward.senderID, reward.senderID, reward.message.Replace("%username%", SaveHandler.GetGameData().username));
            }

            _questIndex++;

            SaveHandler.GetGameData().SaveQuestline(_currentQuest.questID, _questIndex);
            SaveHandler.Save();

            if (_questIndex < _currentQuest.POI_Ids.Length)
            {
                QuestView.Singleton.Paint(_currentQuest, _questIndex);
                onQuestStatusUpdate?.Invoke(_currentQuest, _questIndex);
                UpdateQuest();
            }
            else
            {
                onQuestCompleted?.Invoke(_currentQuest);
                QuestView.Singleton.Hide();
                _currentQuest = null;
            }
        }

        public string GetCurrentQuestID()
        {
            if (_currentQuest == null) return "no_active_quest";

            return _currentQuest.questID;
        }

        public QuestObjectiveData GetCurrentQuestObjective()
        {
            if (_currentQuest == null) return null;

            if (_questIndex >= _currentQuest.POI_Ids.Length) return null;

            return _currentQuest.POI_Ids[_questIndex];
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
                _questIndex = RememberIndex(questData.questID);
            }

            SaveHandler.GetGameData().SaveQuestline(_currentQuest.questID, _questIndex);
            SaveHandler.GetGameData().activatedPOIs.Remove(_currentQuest.POI_Ids[_questIndex].interactionPOI_id); //Se remueve este id para garantizar que sea interactable
            SaveHandler.Save();

            onQuestStatusUpdate?.Invoke(questData, _questIndex);

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

            var poi = POIManager.Instance.GetPOI(_currentQuest.POI_Ids[_questIndex].targetPOI_id);

            _questMark.transform.position = poi.transform.position + Vector3.up * 30;

            QuestView.Singleton.Paint(_currentQuest, _questIndex);
        }

        /// <summary>
        /// Remembers the last visited POI index in the current quest line.
        /// Not yet implemented; currently always returns the first POI (index 0).
        /// </summary>
        /// <returns>The index of the last visited POI (currently always 0).</returns>
        private int RememberIndex(string questlineID)
        {
            var data = SaveHandler.GetGameData();

            if(data.StoredQuestlineExists(questlineID))
            {
                return data.GetStoredIndexQuestline(questlineID);
            }

            return 0;
        }
    }
}
