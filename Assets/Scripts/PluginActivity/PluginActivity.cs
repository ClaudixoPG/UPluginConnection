using System;
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
        string finalMessage = message;

        if (TryParseTelemetry(message, out TelemetryPayload payload))
        {
            payload.receive_ts_unity_ns = GetUnityTimestampNs();
            payload.scene_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            payload.minigame_id = MinigameContext.CurrentMinigameId;

            finalMessage = payload.raw_message;

            if (TelemetryCsvLogger.Instance != null)
            {
                TelemetryCsvLogger.Instance.Log(payload);
            }

            Debug.Log(
                $"[Telemetry] event_id={payload.event_id} " +
                $"type={payload.event_type} " +
                $"family={payload.input_family} " +
                $"sampled={payload.latency_sampled} " +
                $"scene={payload.scene_name} " +
                $"minigame={payload.minigame_id} " +
                $"raw={payload.raw_message}"
            );
        }

        GameObject controllerObject = GameObject.Find("GameController");
        var controller = controllerObject?.GetComponent<IGameController>();
        controller?.HandleMessage(finalMessage);
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

    private bool TryParseTelemetry(string message, out TelemetryPayload payload)
    {
        payload = null;

        if (string.IsNullOrWhiteSpace(message))
            return false;

        try
        {
            payload = JsonUtility.FromJson<TelemetryPayload>(message);

            return payload != null
                   && payload.schema_version > 0
                   && !string.IsNullOrWhiteSpace(payload.raw_message);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private long GetUnityTimestampNs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using var systemClock = new AndroidJavaClass("android.os.SystemClock");
            return systemClock.CallStatic<long>("elapsedRealtimeNanos");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PluginActivity] Failed to get elapsedRealtimeNanos: {e.Message}");
        }
#endif
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000L;
    }
}