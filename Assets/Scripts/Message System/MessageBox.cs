using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MessageSystem
{
    public class MessageBox : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject _messageObject;
        [SerializeField] private GameObject _photoObject;

        [SerializeField] private TextMeshProUGUI _message_text;
        [SerializeField] private Image _photo_image;

        public void SetMessage(string message)
        {
            if (message.Contains("[") && message.Contains("]"))
            {
                if (PhotoExists(message.Replace("[", "").Replace("]", ""), out Sprite photo))
                {
                    DisplayPhoto(photo);
                }
                else
                {
                    DisplayMessage(message);
                }
            }
            else
            {
                DisplayMessage(message);
            }
        }

        private bool PhotoExists(string imageName, out Sprite photoResult)
        {
            var allPhotos = Resources.LoadAll<Sprite>("Message System/Photos");

            foreach (var photo in allPhotos)
            {
                if(imageName == photo.name)
                {
                    photoResult = photo;
                    return true;
                }
            }

            photoResult = null;
            return false;
        }

        private void DisplayMessage(string message)
        {
            _messageObject.SetActive(true);
            _photoObject.SetActive(false);
            _message_text.text = message;
        }

        public void DisplayPhoto(Sprite photo)
        {
            _messageObject.SetActive(false);
            _photoObject.SetActive(true);

            _photo_image.sprite = photo;
        }
    }
}
