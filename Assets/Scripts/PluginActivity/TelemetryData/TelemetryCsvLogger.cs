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
            "scene_name",
            "minigame_id",
            "wear_to_phone_latency_ms",
            "phone_processing_latency_ms",
            "phone_to_unity_latency_ms",
            "end_to_end_latency_ms",
            "test_timestamp_utc",
            "battery_level_watch",
            "temperature_watch_c",
            "battery_level_phone",
            "temperature_phone_c"
        );
    }

    private string BuildRow(TelemetryPayload p)
    {
        double wearToPhoneMs = NsToMs(p.receive_ts_phone_native_ns - p.send_ts_watch_ns);
        double phoneProcessingMs = NsToMs(p.forward_ts_phone_native_ns - p.receive_ts_phone_native_ns);
        double phoneToUnityMs = NsToMs(p.receive_ts_unity_ns - p.forward_ts_phone_native_ns);
        double endToEndMs = NsToMs(p.receive_ts_unity_ns - p.send_ts_watch_ns);

        return string.Join(",",
            Escape(p.event_id),
            Escape(p.session_id),
            Escape(p.event_type),
            Escape(p.input_family),
            Escape(p.raw_message),
            Escape(p.smartwatch_model),
            Escape(p.smartphone_model),
            Escape(p.scene_name),
            Escape(p.minigame_id),

            wearToPhoneMs.ToString("F4", CultureInfo.InvariantCulture),
            phoneProcessingMs.ToString("F4", CultureInfo.InvariantCulture),
            phoneToUnityMs.ToString("F4", CultureInfo.InvariantCulture),
            endToEndMs.ToString("F4", CultureInfo.InvariantCulture),

            Escape(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),

            // ?? batería y temperatura
            p.battery_level_watch.ToString("F2", CultureInfo.InvariantCulture),
            p.temperature_watch_c.ToString("F2", CultureInfo.InvariantCulture),
            p.battery_level_phone.ToString("F2", CultureInfo.InvariantCulture),
            p.temperature_phone_c.ToString("F2", CultureInfo.InvariantCulture)
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