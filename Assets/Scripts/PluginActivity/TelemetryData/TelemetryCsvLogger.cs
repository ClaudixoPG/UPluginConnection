using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class TelemetryCsvLogger : MonoBehaviour
{
    public static TelemetryCsvLogger Instance { get; private set; }

    [SerializeField] private string filePrefix = "telemetry";
    [SerializeField] private bool autoFlushEveryEvent = true;

    private readonly List<TelemetryPayload> _buffer = new();
    private string _filePath;
    private bool _headerWritten;

    public string FilePath => _filePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateFile();
    }

    private void OnApplicationQuit()
    {
        FlushToDisk();
    }

    private void CreateFile()
    {
        var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        _filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(_filePath))
        {
            using var writer = new StreamWriter(_filePath, false, Encoding.UTF8);
            writer.WriteLine(BuildHeader());
        }

        _headerWritten = true;
        Debug.Log($"[TelemetryCsvLogger] CSV path: {_filePath}");
    }

    public void Log(TelemetryPayload payload)
    {
        if (payload == null)
            return;

        _buffer.Add(payload);

        if (autoFlushEveryEvent)
            FlushToDisk();
    }

    public void FlushToDisk()
    {
        if (_buffer.Count == 0)
            return;

        if (!_headerWritten)
        {
            CreateFile();
        }

        using var writer = new StreamWriter(_filePath, true, Encoding.UTF8);

        foreach (var p in _buffer)
        {
            writer.WriteLine(BuildRow(p));
        }

        _buffer.Clear();
    }

    private string BuildHeader()
    {
        return string.Join(",",
            "event_id",
            "session_id",
            "event_type",
            "input_family",
            "raw_message",
            "smartwatch_model",
            "smartphone_model",
            "send_ts_watch_ns",
            "receive_ts_phone_native_ns",
            "forward_ts_phone_native_ns",
            "receive_ts_unity_ns",
            "phone_to_unity_ms",
            "phone_forward_ms",
            "unity_minus_phone_receive_ms",
            "test_timestamp_utc"
        );
    }

    private string BuildRow(TelemetryPayload p)
    {
        double phoneToUnityMs = NsToMs(p.receive_ts_unity_ns - p.forward_ts_phone_native_ns);
        double phoneForwardMs = NsToMs(p.forward_ts_phone_native_ns - p.receive_ts_phone_native_ns);
        double unityMinusPhoneReceiveMs = NsToMs(p.receive_ts_unity_ns - p.receive_ts_phone_native_ns);

        return string.Join(",",
            Escape(p.event_id),
            Escape(p.session_id),
            Escape(p.event_type),
            Escape(p.input_family),
            Escape(p.raw_message),
            Escape(p.smartwatch_model),
            Escape(p.smartphone_model),
            p.send_ts_watch_ns.ToString(CultureInfo.InvariantCulture),
            p.receive_ts_phone_native_ns.ToString(CultureInfo.InvariantCulture),
            p.forward_ts_phone_native_ns.ToString(CultureInfo.InvariantCulture),
            p.receive_ts_unity_ns.ToString(CultureInfo.InvariantCulture),
            phoneToUnityMs.ToString("F4", CultureInfo.InvariantCulture),
            phoneForwardMs.ToString("F4", CultureInfo.InvariantCulture),
            unityMinusPhoneReceiveMs.ToString("F4", CultureInfo.InvariantCulture),
            Escape(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
        );
    }

    private static double NsToMs(long ns)
    {
        return ns / 1_000_000.0;
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}