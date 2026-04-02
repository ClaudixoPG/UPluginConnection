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

            finalMessage = payload.raw_message;

            Debug.Log(
                $"[Telemetry] event_id={payload.event_id} " +
                $"type={payload.event_type} " +
                $"family={payload.input_family} " +
                $"raw={payload.raw_message} " +
                $"watch={payload.send_ts_watch_ns} " +
                $"phone_recv={payload.receive_ts_phone_native_ns} " +
                $"phone_fwd={payload.forward_ts_phone_native_ns} " +
                $"unity_recv={payload.receive_ts_unity_ns}"
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
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000L;
    }
}