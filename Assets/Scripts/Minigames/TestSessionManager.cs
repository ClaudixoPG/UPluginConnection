using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinigameConfig
{
    public string sceneName;
    public string controlName;
    public string minigameId;
}

public class TestSessionManager : MonoBehaviour
{
    public static TestSessionManager Instance { get; private set; }

    [Header("Flow")]
    public List<MinigameConfig> minigames = new();
    public float durationPerMinigame = 10f;
    public float transitionDelay = 1f;
    public float controlSettleDelay = 1.5f;

    [Header("Finish")]
    public string returnSceneName = "TestRunnerScene";
    public bool returnToStartScene = true;
    public bool quitAppWhenFinished = false;

    private bool _running;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartTest()
    {
        if (_running) return;
        _running = true;
        StartCoroutine(RunSession());
    }

    private IEnumerator RunSession()
    {
        for (int i = 0; i < minigames.Count; i++)
        {
            var mg = minigames[i];

            MinigameContext.CurrentMinigameId = mg.minigameId;

            Debug.Log($"[TestSession] Loading: {mg.sceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(mg.sceneName);

            yield return null;

            if (PluginActivity.Instance != null)
            {
                PluginActivity.Instance.UpdateControl(mg.controlName);
                Debug.Log($"[TestSession] Control solicitado: {mg.controlName}");
            }
            else
            {
                Debug.LogWarning("[TestSession] PluginActivity.Instance is null.");
            }

            yield return new WaitForSeconds(controlSettleDelay);

            Debug.Log($"[TestSession] Running {mg.sceneName} for {durationPerMinigame}s");
            yield return new WaitForSeconds(durationPerMinigame);

            Debug.Log($"[TestSession] Finished: {mg.sceneName}");
            yield return new WaitForSeconds(transitionDelay);
        }

        if (TelemetryCsvLogger.Instance != null)
        {
            TelemetryCsvLogger.Instance.FlushToDisk();
            Debug.Log($"[TestSession] CSV saved at: {TelemetryCsvLogger.Instance.FilePath}");
        }

        Debug.Log("[TestSession] TEST FINISHED");

        _running = false;

        if (returnToStartScene && !string.IsNullOrWhiteSpace(returnSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(returnSceneName);
        }
        else if (quitAppWhenFinished)
        {
            Application.Quit();
        }
    }
}