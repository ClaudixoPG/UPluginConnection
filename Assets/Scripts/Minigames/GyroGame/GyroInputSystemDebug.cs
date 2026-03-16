using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GyroInputSystemDebug : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private void Start()
    {
        Debug.Log("Gyroscope.current == null ? " + (UnityEngine.InputSystem.Gyroscope.current == null));

        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
            Debug.Log("Gyroscope enabled: " + UnityEngine.InputSystem.Gyroscope.current.enabled);
        }
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Gyroscope.current == null)
        {
            if (debugText != null)
                debugText.text = "Gyroscope.current = null";
            return;
        }

        Vector3 angularVelocity = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();

        string msg =
            $"gyroEnabled: {UnityEngine.InputSystem.Gyroscope.current.enabled}\n" +
            $"angVel x: {angularVelocity.x:F3}\n" +
            $"angVel y: {angularVelocity.y:F3}\n" +
            $"angVel z: {angularVelocity.z:F3}";

        Debug.Log(msg);

        if (debugText != null)
            debugText.text = msg;
    }

    private void OnDisable()
    {
        if (UnityEngine.InputSystem.Gyroscope.current != null)
            InputSystem.DisableDevice(UnityEngine.InputSystem.Gyroscope.current);
    }
}