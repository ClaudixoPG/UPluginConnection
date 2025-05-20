using UnityEngine;

public abstract class PanelController : MonoBehaviour
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
        CanvasGroup group = GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 0f;
            LeanTween.alphaCanvas(group, 1f, 0.3f);
        }
    }

    public virtual void Hide()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        if (group != null)
        {
            LeanTween.alphaCanvas(group, 0f, 0.3f)
                .setOnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public abstract void HandleMessage(string message);
}
