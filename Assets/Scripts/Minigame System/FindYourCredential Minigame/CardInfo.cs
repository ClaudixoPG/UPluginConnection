using TMPro;
using UnityEngine;

namespace MinigameSystem.Minigames.FindYourCredentials
{
    /// <summary>
    /// Card view displayer
    /// </summary>
    public class CardInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name, _age;

        public void SetCardInfo(string name, string age)
        {
            _name.text = name;
            _age.text = age;
        }
    }
}
