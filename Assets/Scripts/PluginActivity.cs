using TMPro;
using UnityEngine;

public class PluginActivity : MonoBehaviour
{
    //Private Variables
    AndroidJavaObject _pluginActivity;
    //Public Variables
    public TextMeshProUGUI messageToSend;
    public TextMeshProUGUI messageReceived;

    public PanelManager panelManager;


    private int currentControlIndex = 1; // comienza en 1
    private const int maxControls = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pluginActivity = new AndroidJavaObject("com.randomadjective.uactivity.PluginActivity");
    }



    public void Add()
    {
        if (_pluginActivity != null)
        {
            var result = _pluginActivity.Call<int>("Add", 5, 10);
            Debug.Log("Add Output: " + result);
        }
    }
    
    public void ShowToast()
    {
        if (_pluginActivity != null)
        {
            _pluginActivity.Call("ShowToast", "Hello from Unity!");
        }
    }

    public void SendMessage()
    {
        if (_pluginActivity != null)
        {
            var message = messageToSend.text;
            Debug.Log("Message to send: " + message);
            _pluginActivity.Call("sendMessageToSmartwatch", message);
        }
    }

    public void OnMessageReceived(string message)
    {
        Debug.Log("Mensaje recibido en Unity: " + message);
        messageReceived.text = "Mensaje recibido: " + message;
        panelManager.HandleIncomingMessage(message);
    }

    public void NextControl()
    {
        if (_pluginActivity != null)
        {
            currentControlIndex++;
            if (currentControlIndex > maxControls) currentControlIndex = 1;
            UpdateControl();
            /*string message = $"control_{currentControlIndex}";
            Debug.Log("Control -> " + message);
            _pluginActivity.Call("sendMessageToSmartwatch", message);*/

        }
    }

    public void PreviousControl()
    {
        if (_pluginActivity != null)
        {
            currentControlIndex--;
            if (currentControlIndex < 1) currentControlIndex = maxControls;
            UpdateControl();
            /*string message = $"control_{currentControlIndex}";
            Debug.Log("Control -> " + message);
            _pluginActivity.Call("sendMessageToSmartwatch", message);*/
        }
    }

    private void UpdateControl()
    {
        string message = $"control_{currentControlIndex}";
        Debug.Log("Control -> " + message);

        // Enviar al smartwatch
        if (_pluginActivity != null)
        {
            _pluginActivity.Call("sendMessageToSmartwatch", message);
        }

        // Activar el panel correspondiente en Unity (índice - 1)
        panelManager.SetActivePanel(currentControlIndex - 1);
    }

}
