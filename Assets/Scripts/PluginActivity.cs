using TMPro;
using UnityEngine;

public class PluginActivity : MonoBehaviour
{
    //Private Variables
    AndroidJavaObject _pluginActivity;
    //Public Variables
    public TextMeshProUGUI messageToSend;
    public TextMeshProUGUI messageReceived;


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
        messageReceived.text = "Message Received: " + message;
    }
}
