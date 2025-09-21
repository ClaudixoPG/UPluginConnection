using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SaveSystem
{
    public class MinigameStatView : StatView
    {
        [SerializeField] private TextMeshProUGUI _minigameName_text;
        [SerializeField] private Animator _stars_anim;

        private UnityAction _onCompleteCallback;

        public override void Display(StadisticsLog stadistic, UnityAction onCompleteDisplay)
        {
            _onCompleteCallback = onCompleteDisplay;
            _minigameName_text.text = stadistic.log;
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

        private int GetStarsFromValue(float value)
        {
            value = Mathf.Clamp01(value);

            if (value <= 0.1f)
                return 0;
            else if (value <= 0.4f)
                return 1;
            else if (value <= 0.7f)
                return 2;
            else
                return 3;
        }
    }
}
