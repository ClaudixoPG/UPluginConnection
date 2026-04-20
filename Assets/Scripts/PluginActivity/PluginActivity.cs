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
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_ANDROID && !UNITY_EDITOR
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _pluginActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
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

    public void StartMinigameSession(string minigameId)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    if (_pluginActivity != null)
    {
        _pluginActivity.Call("startMinigameSession", minigameId);
    }
#endif
    }

    public void EndMinigameSession()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    if (_pluginActivity != null)
    {
        _pluginActivity.Call("endMinigameSession");
    }
#endif
    }

}

