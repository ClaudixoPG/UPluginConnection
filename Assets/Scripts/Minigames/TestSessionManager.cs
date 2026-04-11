using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinigameConfig
{
    public string sceneName;
    public string controlName;
    public string minigameId;
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

    [Header("Finish")]
    public string returnSceneName = "TestRunnerScene";
    public bool returnToStartScene = true;
    public bool quitAppWhenFinished = false;

    private bool _running;

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

        for (int i = 0; i < minigames.Count; i++)
        {
            var mg = minigames[i];
            Debug.Log($"[TestSessionManager] Starting minigame index={i}, scene={mg.sceneName}, control={mg.controlName}, id={mg.minigameId}");

            MinigameContext.CurrentMinigameId = mg.minigameId;
            MinigameContext.CurrentInstructionText = mg.instructionText;
            MinigameContext.IsMeasurementActive = false;

            MinigameContext.CurrentPhase = "fade_out";
            yield return WaitForFadeOut(overlay);
            Debug.Log("[TestSessionManager] FadeOut done");

            MinigameContext.CurrentPhase = "instructions";
            yield return WaitForInstructions(overlay, mg.instructionText);
            Debug.Log("[TestSessionManager] Instructions done");

            MinigameContext.CurrentPhase = "loading";
            Debug.Log($"[TestSessionManager] Loading scene: {mg.sceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(mg.sceneName);

            yield return null;
            yield return null;

            if (PluginActivity.Instance != null)
            {
                PluginActivity.Instance.UpdateControl(mg.controlName);
                Debug.Log($"[TestSessionManager] Control requested: {mg.controlName}");
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

            yield return new WaitForSeconds(postMinigameDelay);
        }

        MinigameContext.CurrentMinigameId = "";
        MinigameContext.CurrentInstructionText = "";
        MinigameContext.IsMeasurementActive = false;
        MinigameContext.CurrentPhase = "idle";

        if (TelemetryCsvLogger.Instance != null)
        {
            TelemetryCsvLogger.Instance.FlushToDisk();
            Debug.Log($"[TestSessionManager] CSV saved at: {TelemetryCsvLogger.Instance.FilePath}");
        }

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