using HasanSadikin.Carousel;
using UnityEngine;

namespace FindYourCredentials
{
   

    public class CredentialsCarousel : CarouselController<CredentialData>
    {
        public void SetData(CredentialData[] data)
        {
            _data = data;
        }

        private void OnEnable()
        {
            OnItemSelected.AddListener(SelectItem);
            OnCurrentItemUpdated.AddListener(LogItem);
        }

        private void LogItem(CredentialData data)
        {

        }

        private void SelectItem(CredentialData data)
        {
            FindAnyObjectByType<GameController>().currentInspectingData = data;
            FindAnyObjectByType<GameController>().Fire();
        }
    }
}
