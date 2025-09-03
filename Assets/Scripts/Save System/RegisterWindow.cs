using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SaveSystem.Extras.RegisterWindow;

namespace SaveSystem.Extras
{
    /// <summary>
    /// Controls the registration form window.
    /// Validates user input (username and age) 
    /// and creates new save data if valid.
    /// </summary>
    public class RegisterWindow : MonoBehaviour
    {
        [Header("Components")]

        /// <summary>Input field for the player's username.</summary>
        [SerializeField] private TMP_InputField _username_field;

        /// <summary>Input field for the player's age.</summary>
        [SerializeField] private TMP_InputField _age_field;

        /// <summary>Button to continue once inputs are valid.</summary>
        [SerializeField] private Button _continueButton;

        public delegate void OnCompleteRegistration();

        public static event OnCompleteRegistration onCompleteRegistration;

        private void Start()
        {
            ValidateForm();
        }

        public void UI_Validate()
        {
            ValidateForm();
        }

        public void ValidateForm()
        {
            if (!ValidateUsername(_username_field.text))
            {
                _continueButton.interactable = false;
                return;
            }

            if (_age_field.text.Length > 0 && !ValidateAge(int.Parse(_age_field.text)))
            {
                _continueButton.interactable = false;
                return;
            }

            _continueButton.interactable = true;
        }

        /// <summary>
        /// Checks if the username has more than 3 characters.
        /// </summary>
        private bool ValidateUsername(string username)
        {
            return username.Length > 3;
        }

        /// <summary>
        /// Checks if the age is within a valid range (13–116).
        /// </summary>
        private bool ValidateAge(int age)
        {
            return age > 12 && age < 117;
        }

        /// <summary>
        /// Called from the UI button.
        /// Creates new save data and hides the registration window.
        /// </summary>
        public void UI_Continue()
        {
            SaveHandler.CreateNewGameData(_username_field.text, int.Parse(_age_field.text));
            onCompleteRegistration?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
