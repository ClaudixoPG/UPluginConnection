using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypewriterText : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f; // segundos entre caracteres
    private TextMeshProUGUI _textMesh;
    private string _fullText;

    private void OnEnable()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _fullText = _textMesh.text;
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        _textMesh.text = ""; // limpiar al inicio
        foreach (char c in _fullText)
        {
            _textMesh.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
