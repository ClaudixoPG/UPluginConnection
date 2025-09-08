using SpaceShip;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueSceneHandler : MonoBehaviour, IGameController
    {
        [System.Serializable]
        private struct OptionInfo
        {
            public TextMeshProUGUI message_text;

            public GameObject Parent
            {
                get
                {
                    return message_text.transform.parent.gameObject;
                }
            }

            public Button Button
            {
                get
                {
                    return Parent.GetComponent<Button>();
                }
            }
        }

        [Header("Settings")]
        [SerializeField] private float _typingSpeed = 0.05f;

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _messageBox_text;
        [SerializeField] private Image _character_image;
        [SerializeField] private Image _background_image;

        [SerializeField] private OptionInfo[] _options;

        private DialogueModel _currentDialogueModel;

        private int _currentDialogueIndex = -1;
        private float _typingTimer = 0f;
        private string _currentFinalText;
        private string _log;

        public delegate void OnStartDialogue(string dialogueID);
        public delegate void OnEndDialogue(string dialogueID);

        public static event OnStartDialogue onStartDialogue;
        public static event OnEndDialogue onEndDialogue;

        private UnityAction _onEndDialogueAction;

        private void Update()
        {
            if (_currentDialogueModel != null && _currentDialogueIndex < _currentDialogueModel.dialogues.Count)
            {
                TypeDialogueMessageBox(_messageBox_text, _messageBox_text.text, _currentFinalText);
            }
        }

        public void Play(DialogueModel model, UnityAction onEndDailogueAction = null)
        {
            _onEndDialogueAction = onEndDailogueAction;

            _log = $"Init Dialogue: {model.dialogueID}";

            _currentDialogueModel = model;

            _currentDialogueIndex = -1;

            onStartDialogue?.Invoke(model.dialogueID);

            NextDialogue();
        }

        public void HandleMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // --- Tap ---
            if (message.StartsWith("Tap"))
            {
                Skip();
                return;
            }

            // --- Button:A / B / X / Y ---
            if (message.StartsWith("Button:"))
            {
                string button = message.Substring("Button:".Length).ToUpper();
                switch (button)
                {
                    case "A":
                        Skip();
                        break;
                }
                return;
            }
        }

        private void NextDialogue()
        {
            if (_currentDialogueIndex < _currentDialogueModel.dialogues.Count)
            {
                _currentDialogueIndex++;
                _messageBox_text.text = string.Empty;

                if (_currentDialogueIndex == _currentDialogueModel.dialogues.Count)
                {
                    FinishDialogue();
                    return;
                }

                _currentFinalText = _currentDialogueModel.dialogues[_currentDialogueIndex].message.Replace("%username%", SaveSystem.SaveHandler.GetGameData().username);

                if (_character_image.sprite == null || _character_image.sprite != _currentDialogueModel.dialogues[_currentDialogueIndex].character)
                {
                    _character_image.GetComponent<Animator>().SetTrigger("Update");
                    _character_image.sprite = _currentDialogueModel.dialogues[_currentDialogueIndex].character;
                }

                if (_background_image.sprite == null || _background_image.sprite != _currentDialogueModel.dialogues[_currentDialogueIndex].background)
                {
                    _background_image.sprite = _currentDialogueModel.dialogues[_currentDialogueIndex].background;
                }

                UpdateOptions();

            }
            else
            {
                FinishDialogue();
            }
        }

        private void UpdateOptions()
        {
            foreach (var option in _options)
            {
                option.Parent.SetActive(false);
                option.Button.onClick.RemoveAllListeners();
            }

            var dialogueOptions = _currentDialogueModel.dialogues[_currentDialogueIndex].dalogueOptions;

            for (int i = 0; i < dialogueOptions.Count; i++)
            {
                _options[i].Parent.SetActive(true);

                string message = dialogueOptions[i].message;

                _options[i].message_text.text = message;

                _options[i].Button.onClick.AddListener(() => {
                    _log += $"\nChose: {i}/{message}";
                    NextDialogue();
                });
            }
        }

        private void FinishDialogue()
        {
            _log += "\nEnd Dialogue";

            onEndDialogue?.Invoke(_currentDialogueModel.dialogueID);
            _onEndDialogueAction?.Invoke();

            _currentDialogueModel = null;

            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Dialogue Scene");
        }

        public void Skip()
        {
            if (_messageBox_text.text != _currentFinalText)
            {
                _messageBox_text.text = _currentFinalText;
            }
            else
            {
                if (_currentDialogueModel.dialogues[_currentDialogueIndex].dalogueOptions.Count == 0)
                {
                    NextDialogue();
                }
            }
        }

        private void TypeDialogueMessageBox(TextMeshProUGUI messageBox, string currentText, string finalText)
        {
            if (currentText.Length < finalText.Length)
            {
                _typingTimer += Time.deltaTime;

                if (_typingTimer >= _typingSpeed)
                {
                    _typingTimer = 0f;
                    int nextCharIndex = currentText.Length;
                    messageBox.text = finalText.Substring(0, nextCharIndex + 1);
                }
            }
        }
    }
}
