using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // Esto hace que también funcione en el editor
[RequireComponent(typeof(RectTransform))]
public class AutoResizeParent : MonoBehaviour
{
    public TextMeshProUGUI targetText; // El texto que determina la altura
    public float paddingTop = 0f;      // Espacio extra arriba
    public float paddingBottom = 0f;   // Espacio extra abajo

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateHeight();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            UpdateHeight();
        }
#if UNITY_EDITOR
        // Esto permite actualizar también en editor
        if (!Application.isPlaying)
        {
            UpdateHeight();
        }
#endif
    }

    void UpdateHeight()
    {
        if (targetText == null) return;

        // Forzar actualización del layout de TMP
        targetText.ForceMeshUpdate();

        // Obtener la altura requerida del texto
        float textHeight = targetText.textBounds.size.y;

        // Ajustar altura del RectTransform padre
        Vector2 size = rectTransform.sizeDelta;
        size.y = textHeight + paddingTop + paddingBottom;
        rectTransform.sizeDelta = size;
    }
}
