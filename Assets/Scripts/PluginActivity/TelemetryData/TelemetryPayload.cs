using System;

[Serializable]
public class TelemetryPayload
{
    public int schema_version;
    public string event_type;
    public string event_id;
    public string session_id;
    public string input_family;
    public string raw_message;
    public string smartwatch_model;
    public string smartphone_model;
    public string scene_name;
    public string minigame_id;

    public long send_ts_watch_ns;
    public long receive_ts_phone_native_ns;
    public long forward_ts_phone_native_ns;
    public long receive_ts_unity_ns;

    public double battery_level_watch;
    public double temperature_watch_c;
    public double battery_level_phone;
    public double temperature_phone_c;
}