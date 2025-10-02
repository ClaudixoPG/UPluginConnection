using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SaveSystem
{
    public class MinigameStatView : StatView
    {
        [SerializeField] private TextMeshProUGUI _minigameName_text;
        [SerializeField] private TextMeshProUGUI _percentage_text;
        [SerializeField] private Animator _stars_anim;
        [SerializeField] private Image[] _stars;

        private UnityAction _onCompleteCallback;

        public override void Display(StadisticsLog stadistic, UnityAction onCompleteDisplay)
        {
            _onCompleteCallback = onCompleteDisplay;
            _minigameName_text.text = stadistic.log;
            _percentage_text.text = $"{stadistic.percentage * 100}%";

            SetStarsValue(stadistic.percentage, _stars);
            _stars_anim.SetInteger("Stars", GetStarsFromValue(stadistic.percentage));
        }

        private void Update()
        {
            if (_onCompleteCallback != null)
            {
                AnimatorStateInfo state = _stars_anim.GetCurrentAnimatorStateInfo(0);

                if (!state.IsName("0_stars") && state.normalizedTime >= 1f)
                {
                    _onCompleteCallback.Invoke();
                    _onCompleteCallback = null;
                }
            }
        }

        private void SetStarsValue(float value, Image[] stars)
        {
            value = Mathf.Clamp01(value);

            // Convert value (0-1) into stars (0-5)
            float starValue = Mathf.Floor(value * 10f) / 2f; // steps of 0.5

            for (int i = 0; i < stars.Length; i++)
            {
                float remaining = starValue - i;

                if (remaining >= 1f)
                {
                    stars[i].fillAmount = 1f; // full star
                }
                else if (remaining >= 0.5f)
                {
                    stars[i].fillAmount = 0.5f; // half star
                }
                else
                {
                    stars[i].fillAmount = 0f; // empty
                }
            }
        }

        private int GetStarsFromValue(float value)
        {
            value = Mathf.Clamp01(value);

            // Convert to star range (0-5) with half steps
            float rawStars = Mathf.Round(value * 10f) / 2f; // gives steps of 0.5

            // Truncate .5 values 
            int stars = Mathf.FloorToInt(rawStars);

            return stars;
        }
    }
}
