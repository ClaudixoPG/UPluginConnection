using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoystickPanelController : PanelController
{
    public TextMeshProUGUI joystickText;
    public RectTransform stick;
    public float movementRange = 100f; // máximo desplazamiento visual

    void Start()
    {
        if (stick != null && stick.parent is RectTransform parent)
        {
            movementRange = parent.rect.width * 0.5f; // o usa height si es cuadrado
        }
    }


    public override void HandleMessage(string message)
    {
        if (!message.StartsWith("joystick:")) return;

        string[] partes = message.Replace("joystick:", "").Split(',');
        if (partes.Length != 2) return;

        if (float.TryParse(partes[0], out float x) && float.TryParse(partes[1], out float y))
        {
            joystickText.text = $"X: {x:0.00} / Y: {y:0.00}";

            Vector2 targetPosition = new Vector2(x, y) * movementRange;
            stick.anchoredPosition = targetPosition;

            // Animación de retorno al centro si es (0,0)
            if (Mathf.Approximately(x, 0f) && Mathf.Approximately(y, 0f))
            {
                LeanTween.move(stick, Vector2.zero, 0.2f).setEaseOutQuad();
            }
        }
    }
}
