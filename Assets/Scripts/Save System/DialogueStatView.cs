using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SaveSystem
{
    public class DialogueStatView : StatView
    {
        [SerializeField] private TextMeshProUGUI _title_text;

        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _dialogue_prefab;

        public override void Display(StadisticsLog stadistic, UnityAction onCompleteDisplay)
        {
            _title_text.text = stadistic.stadisticName.Split('/')[1];

            Tuple<string, string, string>[] dialogues = ParseLogToDialogues(stadistic.log);

            StartCoroutine(DisplayDialogues(dialogues, onCompleteDisplay));
        }

        private IEnumerator DisplayDialogues(Tuple<string, string, string>[] dialogues, UnityAction onCompleteCallback)
        {
            foreach (var dialogue in dialogues)
            {
                var dialogue_GO = Instantiate(_dialogue_prefab, _content);

                dialogue_GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogue.Item1;
                dialogue_GO.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{dialogue.Item2}. {dialogue.Item3}";

                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();

                yield return new WaitForSeconds(0.5f);
            }

            onCompleteCallback?.Invoke();
        }

        private Tuple<string, string, string>[] ParseLogToDialogues(string log)
        {
            List<Tuple<string, string, string>> dialoguesList = new List<Tuple<string, string, string>>();

            // Dividir en bloques entre [ y ]
            string[] blocks = log.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in blocks)
            {
                // Separar cada bloque por "/"
                string[] parts = block.Split('/');
                if (parts.Length == 3)
                {
                    dialoguesList.Add(Tuple.Create(parts[0], parts[1], parts[2]));
                }
            }

            return dialoguesList.ToArray();
        }
    }
}
