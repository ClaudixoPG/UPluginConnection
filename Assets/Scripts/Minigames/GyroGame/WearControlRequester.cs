using UnityEngine;

namespace GyroMiniGame
{
    public class WearControlRequester : MonoBehaviour
    {
        [SerializeField] private bool requestOnStart = true;
        [SerializeField] private string controlName = "sensor_gyro";

        private void Start()
        {
            if (!requestOnStart)
                return;

            if (PluginActivity.Instance != null)
            {
                PluginActivity.Instance.UpdateControl(controlName);
                Debug.Log($"[WearControlRequester] Control solicitado: {controlName}");
            }
            else
            {
                Debug.LogWarning("[WearControlRequester] PluginActivity.Instance es null.");
            }
        }
    }
}