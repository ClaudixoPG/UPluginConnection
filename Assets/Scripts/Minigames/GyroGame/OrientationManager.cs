using UnityEngine;

namespace GyroMiniGame
{
    public class OrientationManager : MonoBehaviour
    {
        private ScreenOrientation _previousOrientation;
        private bool _prevPortrait;
        private bool _prevPortraitUpsideDown;
        private bool _prevLandscapeLeft;
        private bool _prevLandscapeRight;

        private void Start()
        {
            _previousOrientation = Screen.orientation;
            _prevPortrait = Screen.autorotateToPortrait;
            _prevPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown;
            _prevLandscapeLeft = Screen.autorotateToLandscapeLeft;
            _prevLandscapeRight = Screen.autorotateToLandscapeRight;

            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = false;
        }

        private void OnDestroy()
        {
            Screen.orientation = _previousOrientation;
            Screen.autorotateToPortrait = _prevPortrait;
            Screen.autorotateToPortraitUpsideDown = _prevPortraitUpsideDown;
            Screen.autorotateToLandscapeLeft = _prevLandscapeLeft;
            Screen.autorotateToLandscapeRight = _prevLandscapeRight;
        }
    }
}