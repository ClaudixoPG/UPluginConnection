using UnityEngine;

public class PluginActivity : MonoBehaviour
{
    private AndroidJavaObject _pluginActivity;

    public static PluginActivity Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_ANDROID && !UNITY_EDITOR
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _pluginActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (_pluginActivity != null)
        {
            Debug.Log("[PluginActivity] currentActivity inicializado en Awake()");
        }
        else
        {
            Debug.LogWarning("[PluginActivity] currentActivity es null en Awake()");
        }
#endif
    }

    public void OnMessageReceived(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (!MinigameContext.IsMeasurementActive) return;

        GameObject controllerObject = GameObject.Find("GameController");
        var controller = controllerObject?.GetComponent<IGameController>();
        controller?.HandleMessage(message);
    }

    public void UpdateControl(int controlIndex)
    {
        SendMessageToSmartwatch($"control_{controlIndex}");
    }

    public void UpdateControl(string controlName)
    {
        if (string.IsNullOrWhiteSpace(controlName))
        {
            Debug.LogWarning("[PluginActivity] controlName vacío.");
            return;
        }

        SendMessageToSmartwatch(controlName);
    }

    public void SendMessageToSmartwatch(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginActivity != null)
        {
            Debug.Log($"[PluginActivity] sendMessageToSmartwatch -> {message}");
            _pluginActivity.Call("sendMessageToSmartwatch", message);
        }
        else
        {
            Debug.LogWarning("[PluginActivity] _pluginActivity es null.");
        }
#else
        Debug.Log($"[PluginActivity] Mensaje a smartwatch: {message}");
#endif
    }

    public string GetPhoneSessionSnapshot()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginActivity != null)
        {
            return _pluginActivity.Call<string>("getPhoneSessionSnapshot");
        }
#endif
        return string.Empty;
    }

    public void StartTestSession()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginActivity != null)
        {
            Debug.Log("[PluginActivity] startTestSession()");
            _pluginActivity.Call("startTestSession");
        }
        else
        {
            Debug.LogWarning("[PluginActivity] _pluginActivity es null on StartTestSession.");
        }
#else
        Debug.Log("[PluginActivity] startTestSession()");
#endif
    }

    public void EndTestSession()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginActivity != null)
        {
            Debug.Log("[PluginActivity] endTestSession()");
            _pluginActivity.Call("endTestSession");
        }
        else
        {
            Debug.LogWarning("[PluginActivity] _pluginActivity es null on EndTestSession.");
        }
#else
        Debug.Log("[PluginActivity] endTestSession()");
#endif
    }

    public void StartMinigameSession(string minigameId)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginActivity != null)
        {
            Debug.Log($"[PluginActivity] startMinigameSession({minigameId})");
            _pluginActivity.Call("startMinigameSession", minigameId);
        }
        else
        {
            Debug.LogWarning("[PluginActivity] _pluginActivity es null on StartMinigameSession.");
        }
#else
        Debug.Log($"[PluginActivity] startMinigameSession({minigameId})");
#endif
    }

    public void EndMinigameSession()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginActivity != null)
        {
            Debug.Log("[PluginActivity] endMinigameSession()");
            _pluginActivity.Call("endMinigameSession");
        }
        else
        {
            Debug.LogWarning("[PluginActivity] _pluginActivity es null on EndMinigameSession.");
        }
#else
        Debug.Log("[PluginActivity] endMinigameSession()");
#endif
    }
}