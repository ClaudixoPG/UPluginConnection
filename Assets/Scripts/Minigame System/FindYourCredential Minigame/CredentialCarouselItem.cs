using DG.Tweening;
using HasanSadikin.Carousel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FindYourCredentials
{
    public class CredentialData
    {
        public int color;
        public string name;
        public string age;
        public bool isCorrect;

        public CredentialData(int color, string name, string age)
        {
            this.color = color;
            this.name = name;
            this.age = age;
        }
    }

    public class CredentialCarouselItem : CarouselItem<CredentialData>
    {
        [SerializeField] private Image _card;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _age;

        [SerializeField] private Sprite[] _cardSprites;

        protected override void OnDataUpdated(CredentialData data)
        {
            _card.sprite = _cardSprites[(int)Mathf.Repeat(data.color, _cardSprites.Length)];
            _name.text = data.name;
            _age.text = data.age;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            this.CreateSequence()
                .Join(_rectTransform.DOScale(1.2f, 0.25f));
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.CreateSequence()
                .Join(_rectTransform.DOScale(1, 0.25f));
        }
    }
}
