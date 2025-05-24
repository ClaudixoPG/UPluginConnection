using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForcePanelController : PanelController
{
    public TextMeshProUGUI forceText;
    public Slider forceSlider;

    public override void HandleMessage(string message)
    {
        if (!message.StartsWith("fuerza:")) return;

        string valor = message.Replace("fuerza:", "");
        if (float.TryParse(valor, out float fuerza))
        {
            fuerza = Mathf.Clamp01(fuerza);

            forceText.text = $"Fuerza: {fuerza:0.00}";

            // Animación suave de la barra
            float valorActual = forceSlider.value;
            LeanTween.value(gameObject, valorActual, fuerza, 0.25f)
                .setOnUpdate((float val) => forceSlider.value = val);
        }
    }
}