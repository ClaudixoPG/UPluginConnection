using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinigameScreenOrientation
{
    Portrait,
    LandscapeLeft
}

[System.Serializable]
public class MinigameConfig
{
    public string sceneName;
    public string controlName;
    public string minigameId;
    public MinigameScreenOrientation screenOrientation = MinigameScreenOrientation.Portrait;
    [TextArea(2, 5)] public string instructionText;
}

public class TestSessionManager : MonoBehaviour
{
    public static TestSessionManager Instance { get; private set; }

    [Header("Flow")]
    public List<MinigameConfig> minigames = new();
    public float durationPerMinigame = 10f;
    public float controlSettleDelay = 1.5f;
    public float postMinigameDelay = 0.5f;

    [Header("Transition UI")]
    public float fadeOutDuration = 0.5f;
    public float instructionDuration = 20f;
    public float fadeInDuration = 0.5f;
    public int countdownStart = 3;
    public float countdownStepDuration = 1f;

    [Header("Orientation")]
    public float orientationSettleDelay = 0.5f;

    [Header("Finish")]
    public string returnSceneName = "TestRunnerScene";
    public bool returnToStartScene = true;
    public bool quitAppWhenFinished = false;

    private bool _running;

    private ScreenOrientation _previousOrientation;
    private bool _prevPortrait;
    private bool _prevPortraitUpsideDown;
    private bool _prevLandscapeLeft;
    private bool _prevLandscapeRight;
    private bool _screenStateCaptured;

    private void Awake()
    {
        Debug.Log("[TestSessionManager] Awake()");

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[TestSessionManager] Duplicate instance detected. Destroying this object.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        MinigameContext.CurrentMinigameId = "";
        MinigameContext.IsMeasurementActive = false;
        MinigameContext.CurrentPhase = "idle";
        MinigameContext.CurrentInstructionText = "";

        Debug.Log("[TestSessionManager] Awake() completed");
    }

    public void StartTest()
    {
        Debug.Log("[TestSessionManager] StartTest() called");

        if (_running)
        {
            Debug.LogWarning("[TestSessionManager] Test already running.");
            return;
        }

        if (minigames == null || minigames.Count == 0)
        {
            Debug.LogError("[TestSessionManager] No minigames configured.");
            return;
        }

        CaptureCurrentScreenState();

        _running = true;
        StartCoroutine(RunSession());
    }

    private IEnumerator RunSession()
    {
        Debug.Log("[TestSessionManager] RunSession() started");

        var overlay = TransitionOverlayUI.Instance;
        if (overlay == null)
        {
            Debug.LogError("[TestSessionManager] TransitionOverlayUI.Instance is null.");
            _running = false;
            yield break;
        }

        overlay.HideAllImmediate();
        Debug.Log("[TestSessionManager] Overlay ready");

        if (PluginActivity.Instance != null)
        {
            PluginActivity.Instance.StartTestSession();
            Debug.Log("[TestSessionManager] TEST_SESSION_START sent");
        }
        else
        {
            Debug.LogWarning("[TestSessionManager] PluginActivity.Instance is null on StartTestSession.");
        }

        for (int i = 0; i < minigames.Count; i++)
        {
            var mg = minigames[i];
            Debug.Log($"[TestSessionManager] Starting minigame index={i}, scene={mg.sceneName}, control={mg.controlName}, id={mg.minigameId}, orientation={mg.screenOrientation}");

            MinigameContext.CurrentMinigameId = mg.minigameId;
            MinigameContext.CurrentInstructionText = mg.instructionText;
            MinigameContext.IsMeasurementActive = false;

            MinigameContext.CurrentPhase = "fade_out";
            yield return WaitForFadeOut(overlay);
            Debug.Log("[TestSessionManager] FadeOut done");

            MinigameContext.CurrentPhase = "instructions";
            yield return WaitForInstructions(overlay, mg.instructionText);
            Debug.Log("[TestSessionManager] Instructions done");

            ApplyOrientation(mg.screenOrientation);
            yield return new WaitForSeconds(orientationSettleDelay);

            MinigameContext.CurrentPhase = "loading";
            Debug.Log($"[TestSessionManager] Loading scene: {mg.sceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(mg.sceneName);

            yield return null;
            yield return null;

            if (PluginActivity.Instance != null)
            {
                PluginActivity.Instance.UpdateControl(mg.controlName);
                PluginActivity.Instance.StartMinigameSession(mg.minigameId);
                Debug.Log($"[TestSessionManager] Control requested: {mg.controlName}");
                Debug.Log($"[TestSessionManager] Minigame session started: {mg.minigameId}");
            }
            else
            {
                Debug.LogWarning("[TestSessionManager] PluginActivity.Instance is null.");
            }

            yield return new WaitForSeconds(controlSettleDelay);

            MinigameContext.CurrentPhase = "fade_in";
            yield return WaitForFadeIn(overlay);
            Debug.Log("[TestSessionManager] FadeIn done");

            MinigameContext.CurrentPhase = "countdown";
            yield return WaitForCountdown(overlay);
            Debug.Log("[TestSessionManager] Countdown done");

            MinigameContext.CurrentPhase = "active";
            MinigameContext.IsMeasurementActive = true;

            Debug.Log($"[TestSessionManager] Running {mg.sceneName} for {durationPerMinigame}s");
            yield return new WaitForSeconds(durationPerMinigame);

            MinigameContext.IsMeasurementActive = false;
            MinigameContext.CurrentPhase = "fade_out";
            yield return WaitForFadeOut(overlay);
            Debug.Log($"[TestSessionManager] Finished minigame: {mg.sceneName}");

            if (PluginActivity.Instance != null)
            {
                PluginActivity.Instance.EndMinigameSession();
                Debug.Log($"[TestSessionManager] Minigame session ended and saved: {mg.minigameId}");
            }
            else
            {
                Debug.LogWarning("[TestSessionManager] PluginActivity.Instance is null on EndMinigameSession.");
            }

            yield return new WaitForSeconds(postMinigameDelay);
        }

        if (PluginActivity.Instance != null)
        {
            PluginActivity.Instance.EndTestSession();
            Debug.Log("[TestSessionManager] TEST_SESSION_END sent");
        }
        else
        {
            Debug.LogWarning("[TestSessionManager] PluginActivity.Instance is null on EndTestSession.");
        }

        RestorePreviousScreenState();

        MinigameContext.CurrentMinigameId = "";
        MinigameContext.CurrentInstructionText = "";
        MinigameContext.IsMeasurementActive = false;
        MinigameContext.CurrentPhase = "idle";

        Debug.Log("[TestSessionManager] TEST FINISHED");

        _running = false;
        TransitionOverlayUI.Instance.HideAllImmediate();

        if (returnToStartScene && !string.IsNullOrWhiteSpace(returnSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(returnSceneName);
        }
        else if (quitAppWhenFinished)
        {
            Application.Quit();
        }
    }

    private void CaptureCurrentScreenState()
    {
        if (_screenStateCaptured) return;

        _previousOrientation = Screen.orientation;
        _prevPortrait = Screen.autorotateToPortrait;
        _prevPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown;
        _prevLandscapeLeft = Screen.autorotateToLandscapeLeft;
        _prevLandscapeRight = Screen.autorotateToLandscapeRight;
        _screenStateCaptured = true;

        Debug.Log("[TestSessionManager] Screen state captured");
    }

    private void ApplyOrientation(MinigameScreenOrientation orientation)
    {
        switch (orientation)
        {
            case MinigameScreenOrientation.Portrait:
                Screen.orientation = ScreenOrientation.Portrait;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                Debug.Log("[TestSessionManager] Orientation applied: Portrait");
                break;

            case MinigameScreenOrientation.LandscapeLeft:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = false;
                Debug.Log("[TestSessionManager] Orientation applied: LandscapeLeft");
                break;
        }
    }

    private void RestorePreviousScreenState()
    {
        if (!_screenStateCaptured) return;

        Screen.orientation = _previousOrientation;
        Screen.autorotateToPortrait = _prevPortrait;
        Screen.autorotateToPortraitUpsideDown = _prevPortraitUpsideDown;
        Screen.autorotateToLandscapeLeft = _prevLandscapeLeft;
        Screen.autorotateToLandscapeRight = _prevLandscapeRight;

        Debug.Log("[TestSessionManager] Screen state restored");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            RestorePreviousScreenState();
        }
    }

    private IEnumerator WaitForFadeOut(TransitionOverlayUI overlay)
    {
        bool done = false;
        overlay.FadeOut(fadeOutDuration, () => done = true);
        yield return new WaitUntil(() => done);
    }

    private IEnumerator WaitForInstructions(TransitionOverlayUI overlay, string message)
    {
        bool done = false;
        overlay.ShowInstruction(message, instructionDuration, () => done = true);
        yield return new WaitUntil(() => done);
    }

    private IEnumerator WaitForFadeIn(TransitionOverlayUI overlay)
    {
        bool done = false;
        overlay.FadeIn(fadeInDuration, () => done = true);
        yield return new WaitUntil(() => done);
    }

    private IEnumerator WaitForCountdown(TransitionOverlayUI overlay)
    {
        bool done = false;
        overlay.ShowCountdown(countdownStart, countdownStepDuration, () => done = true);
        yield return new WaitUntil(() => done);
    }
}