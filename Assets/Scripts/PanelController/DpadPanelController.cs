using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DpadPanelController : PanelController
{
    public Image upArrow;
    public Image downArrow;
    public Image leftArrow;
    public Image rightArrow;
    public TextMeshProUGUI feedbackText;

    public Color highlightColor = new Color(1f, 1f, 0f, 0.5f); // Amarillo translúcido
    private Color defaultColor;

    private void Start()
    {
        // Se asume que todos los botones tienen el mismo color base
        defaultColor = upArrow.color;

        // Por si no está seteado
        if (feedbackText != null)
            feedbackText.text = "Waiting Text...";
    }

    /*DEBUG
    private void Update()
    {
        //four directions
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Highlight(upArrow);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Highlight(downArrow);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Highlight(leftArrow);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Highlight(rightArrow);
        }
    }
    */

    public override void HandleMessage(string message)
    {
        ResetColors();

        if (feedbackText != null)
            feedbackText.text = message;

        switch (message.ToLower())
        {
            case "arriba":
                Highlight(upArrow);
                break;
            case "abajo":
                Highlight(downArrow);
                break;
            case "izquierda":
                Highlight(leftArrow);
                break;
            case "derecha":
                Highlight(rightArrow);
                break;
        }
    }

    private void Highlight(Image arrow)
    {
        arrow.color = highlightColor;
        LeanTween.color(arrow.rectTransform, defaultColor, 0.4f);
    }

    private void ResetColors()
    {
        upArrow.color = defaultColor;
        downArrow.color = defaultColor;
        leftArrow.color = defaultColor;
        rightArrow.color = defaultColor;
    }
}
