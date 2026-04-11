public static class MinigameContext
{
    public static string CurrentMinigameId = "";
    public static bool IsMeasurementActive = false;
    public static string CurrentPhase = "idle"; // idle, fade_out, instructions, loading, fade_in, countdown, active
    public static string CurrentInstructionText = "";
}