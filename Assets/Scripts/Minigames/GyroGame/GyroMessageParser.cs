using System.Globalization;
using UnityEngine;

namespace GyroMiniGame
{
    public static class GyroMessageParser
    {
        public static bool TryParseRaw(string message, out Vector3 gyro)
        {
            gyro = Vector3.zero;

            if (string.IsNullOrEmpty(message))
                return false;

            if (!message.StartsWith("GyroRaw:"))
                return false;

            string payload = message.Substring("GyroRaw:".Length);
            string[] parts = payload.Split(',');

            if (parts.Length != 3)
                return false;

            if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                return false;

            if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                return false;

            if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                return false;

            gyro = new Vector3(x, y, z);
            return true;
        }
    }
}