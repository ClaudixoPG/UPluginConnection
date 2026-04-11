using UnityEngine;

public class InstructionsOverlayBootstrap : MonoBehaviour
{
    private void Awake()
    {
        _ = TransitionOverlayUI.Instance;
    }
}