using System.Collections;
using UnityEngine;

public class TimedTelemetrySession : MonoBehaviour
{
    [SerializeField] private float durationSeconds = 300f; // 5 min
    [SerializeField] private bool startOnAwake = true;

    private Coroutine _sessionCoroutine;
    private bool _running;

    private void Start()
    {
        if (startOnAwake)
        {
            StartTimedSession();
        }
    }

    public void StartTimedSession()
    {
        if (_running)
            return;

        _sessionCoroutine = StartCoroutine(RunSession());
    }

    private IEnumerator RunSession()
    {
        _running = true;

        Debug.Log($"[TimedTelemetrySession] Session started. Duration: {durationSeconds} seconds");

        yield return new WaitForSeconds(durationSeconds);

        if (TelemetryCsvLogger.Instance != null)
        {
            TelemetryCsvLogger.Instance.FlushToDisk();
            Debug.Log($"[TimedTelemetrySession] Session finished. CSV saved at: {TelemetryCsvLogger.Instance.FilePath}");
        }
        else
        {
            Debug.LogWarning("[TimedTelemetrySession] TelemetryCsvLogger.Instance is null.");
        }

        _running = false;
    }
}