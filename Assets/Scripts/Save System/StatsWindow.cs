using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SaveSystem
{
    public class StatsWindow : MonoBehaviour
    {
        [SerializeField] private Transform _statsContent;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Scrollbar _verticalScrollbar;
        [SerializeField] private StatView _minigameStat, _dialogueStat;

        private Queue<StadisticsLog> _stadisticsLog;

        public void DisplayStats(StadisticsLog[] stadistics)
        {
            gameObject.SetActive(true);

            _verticalScrollbar.gameObject.SetActive(false);

            _stadisticsLog = new Queue<StadisticsLog>(stadistics);

            NextStat();
        }

        private void NextStat()
        {
            if (_stadisticsLog.Count > 0)
            {
                var stadistic = _stadisticsLog.Dequeue();

                StatView statView = null;

                switch (GetTag(stadistic.stadisticName))
                {
                    case "[MINIGAME]":
                        statView = Instantiate(_minigameStat, _statsContent);
                        break;
                    case "[DIALOGUE]":
                        statView = Instantiate(_dialogueStat, _statsContent);
                        break;
                }

                statView.Display(stadistic, NextStat);
            }
            else
            {
                Finish();
            }
        }

        private void Finish()
        {
            _verticalScrollbar.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_stadisticsLog != null && _stadisticsLog.Count > 0)
            {
                Canvas.ForceUpdateCanvases();
                _scrollRect.verticalNormalizedPosition = Mathf.Lerp(_scrollRect.verticalNormalizedPosition, 0, Time.deltaTime * 4f);
            }
        }

        private string GetTag(string input)
        {
            int start = input.IndexOf('[');
            int end = input.IndexOf(']');

            if (start >= 0 && end > start)
                return input.Substring(start, end - start + 1); // incluye []

            return string.Empty;
        }
    }
}
