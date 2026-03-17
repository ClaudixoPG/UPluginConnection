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
#if UNITY_ANDROID && !UNITY_EDITOR
        _pluginActivity = new AndroidJavaObject("com.randomadjective.uactivity.PluginActivity");
#endif
    }

    public void ShowToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_pluginActivity != null)
        {
            _pluginActivity.Call("ShowToast", message);
        }
#else
        Debug.Log($"[PluginActivity] Toast: {message}");
#endif
    }

    public void OnMessageReceived(string message)
    {
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
}