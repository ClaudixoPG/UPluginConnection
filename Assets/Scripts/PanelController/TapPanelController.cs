using TMPro;
using UnityEngine;

public class TapPanelController : PanelController
{
    public TextMeshProUGUI feedbackText;

    public override void HandleMessage(string message)
    {
        if (message == "Shoot")
        {
            feedbackText.text = "¡Disparo!";
            feedbackText.alpha = 1f;
            LeanTween.alphaText(feedbackText.rectTransform, 0f, 0.5f);
        }
    }
}
