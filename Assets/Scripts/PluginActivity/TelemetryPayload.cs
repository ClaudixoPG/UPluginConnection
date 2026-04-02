using System;
using UnityEngine;

[Serializable]
public class TelemetryPayload
{
    public int schema_version;
    public string event_type;
    public string event_id;
    public string session_id;
    public string input_family;
    public string raw_message;
    public long send_ts_watch_ns;
    public string smartwatch_model;
    public long receive_ts_phone_native_ns;
    public long forward_ts_phone_native_ns;
    public long receive_ts_unity_ns;
    public string smartphone_model;
}