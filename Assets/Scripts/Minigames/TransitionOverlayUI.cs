using System;
using UnityEngine;
using UnityEngine.UI;

public class TransitionOverlayUI : MonoBehaviour
{
    private static TransitionOverlayUI _instance;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private Image _background;
    private Text _instructionText;
    private Text _countdownText;

    public static TransitionOverlayUI Instance
    {
        get
        {
            EnsureInstance();
            return _instance;
        }
    }

    private void Awake()
    {
        Debug.Log("[TransitionOverlayUI] Awake()");

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        BuildUI();
        HideAllImmediate();

        Debug.Log("[TransitionOverlayUI] Awake() completed");
    }

    private static void EnsureInstance()
    {
        if (_instance != null) return;

        Debug.Log("[TransitionOverlayUI] Creating singleton instance");
        var go = new GameObject("TransitionOverlayUI");
        _instance = go.AddComponent<TransitionOverlayUI>();
    }

    private void BuildUI()
    {
        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 9999;

        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        var defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(transform, false);

        _background = backgroundObj.AddComponent<Image>();
        _background.color = new Color(0f, 0f, 0f, 0f);
        _background.raycastTarget = false;

        var bgRect = _background.rectTransform;
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        var instructionObj = new GameObject("InstructionText");
        instructionObj.transform.SetParent(transform, false);

        _instructionText = instructionObj.AddComponent<Text>();
        _instructionText.font = defaultFont;
        _instructionText.fontSize = 34;
        _instructionText.alignment = TextAnchor.MiddleCenter;
        _instructionText.color = Color.white;
        _instructionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _instructionText.verticalOverflow = VerticalWrapMode.Overflow;
        _instructionText.raycastTarget = false;

        var instructionRect = _instructionText.rectTransform;
        instructionRect.anchorMin = new Vector2(0.08f, 0.2f);
        instructionRect.anchorMax = new Vector2(0.92f, 0.8f);
        instructionRect.offsetMin = Vector2.zero;
        instructionRect.offsetMax = Vector2.zero;

        var countdownObj = new GameObject("CountdownText");
        countdownObj.transform.SetParent(transform, false);

        _countdownText = countdownObj.AddComponent<Text>();
        _countdownText.font = defaultFont;
        _countdownText.fontSize = 96;
        _countdownText.alignment = TextAnchor.MiddleCenter;
        _countdownText.color = Color.white;
        _countdownText.horizontalOverflow = HorizontalWrapMode.Overflow;
        _countdownText.verticalOverflow = VerticalWrapMode.Overflow;
        _countdownText.raycastTarget = false;

        var countdownRect = _countdownText.rectTransform;
        countdownRect.anchorMin = new Vector2(0.25f, 0.35f);
        countdownRect.anchorMax = new Vector2(0.75f, 0.65f);
        countdownRect.offsetMin = Vector2.zero;
        countdownRect.offsetMax = Vector2.zero;
    }

    public void HideAllImmediate()
    {
        if (gameObject != null)
        {
            LeanTween.cancel(gameObject);
        }

        SetBackgroundAlpha(0f);

        if (_instructionText != null)
        {
            _instructionText.gameObject.SetActive(false);
            _instructionText.text = "";
        }

        if (_countdownText != null)
        {
            _countdownText.gameObject.SetActive(false);
            _countdownText.text = "";
            _countdownText.rectTransform.localScale = Vector3.one;
        }

        SetInputBlocking(false);
    }

    public void FadeOut(float duration, Action onComplete = null)
    {
        Debug.Log("[TransitionOverlayUI] FadeOut()");
        LeanTween.cancel(gameObject);

        _instructionText.gameObject.SetActive(false);
        _countdownText.gameObject.SetActive(false);

        SetInputBlocking(true);

        LeanTween.value(gameObject, _background.color.a, 1f, duration)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float alpha) => SetBackgroundAlpha(alpha))
            .setOnComplete(() =>
            {
                Debug.Log("[TransitionOverlayUI] FadeOut complete");
                onComplete?.Invoke();
            });
    }

    public void FadeIn(float duration, Action onComplete = null)
    {
        Debug.Log("[TransitionOverlayUI] FadeIn()");
        LeanTween.cancel(gameObject);

        _instructionText.gameObject.SetActive(false);
        _countdownText.gameObject.SetActive(false);

        SetInputBlocking(true);

        LeanTween.value(gameObject, _background.color.a, 0f, duration)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float alpha) => SetBackgroundAlpha(alpha))
            .setOnComplete(() =>
            {
                Debug.Log("[TransitionOverlayUI] FadeIn complete");
                SetInputBlocking(false);
                onComplete?.Invoke();
            });
    }

    public void ShowInstruction(string message, float duration, Action onComplete = null)
    {
        Debug.Log("[TransitionOverlayUI] ShowInstruction()");

        _countdownText.gameObject.SetActive(false);
        _instructionText.gameObject.SetActive(true);
        _instructionText.text = message;

        SetInputBlocking(true);

        LeanTween.delayedCall(gameObject, duration, () =>
        {
            _instructionText.gameObject.SetActive(false);
            Debug.Log("[TransitionOverlayUI] ShowInstruction complete");
            onComplete?.Invoke();
        }).setIgnoreTimeScale(true);
    }

    public void ShowCountdown(int startValue, float stepDuration, Action onComplete = null)
    {
        Debug.Log("[TransitionOverlayUI] ShowCountdown()");
        _instructionText.gameObject.SetActive(false);
        _countdownText.gameObject.SetActive(true);

        SetInputBlocking(true);
        RunCountdownStep(startValue, stepDuration, onComplete);
    }

    private void RunCountdownStep(int value, float stepDuration, Action onComplete)
    {
        string label = value > 0 ? value.ToString() : "¡Ya!";
        _countdownText.text = label;
        _countdownText.rectTransform.localScale = Vector3.one * 0.6f;

        LeanTween.scale(_countdownText.rectTransform, Vector3.one, stepDuration * 0.5f)
            .setIgnoreTimeScale(true)
            .setEaseOutBack();

        LeanTween.delayedCall(gameObject, stepDuration, () =>
        {
            if (value > 1)
            {
                RunCountdownStep(value - 1, stepDuration, onComplete);
                return;
            }

            if (value == 1)
            {
                RunCountdownStep(0, stepDuration, onComplete);
                return;
            }

            _countdownText.gameObject.SetActive(false);
            SetInputBlocking(false);
            Debug.Log("[TransitionOverlayUI] ShowCountdown complete");
            onComplete?.Invoke();
        }).setIgnoreTimeScale(true);
    }

    private void SetBackgroundAlpha(float alpha)
    {
        var c = _background.color;
        c.a = Mathf.Clamp01(alpha);
        _background.color = c;
    }

    private void SetInputBlocking(bool shouldBlock)
    {
        if (_canvasGroup == null) return;

        _canvasGroup.blocksRaycasts = shouldBlock;
        _canvasGroup.interactable = shouldBlock;
    }
}