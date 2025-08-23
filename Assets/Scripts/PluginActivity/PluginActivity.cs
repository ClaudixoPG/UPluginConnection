using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PluginActivity : MonoBehaviour
{
    //Private Variables
    AndroidJavaObject _pluginActivity;
    
    //Public Variables
    //public TextMeshProUGUI messageToSend;
    //public TextMeshProUGUI messageReceived;

    //public PanelManager panelManager;


    private int currentControlIndex = 1; // comienza en 1
    //private int maxControls = 2;

    void Awake()
    {
        //if (FindObjectsOfType<PluginActivity>().Length > 1)
        if(FindFirstObjectByType<PluginActivity>() != this)
        {
            Destroy(gameObject); // evitar duplicados si vuelves a la escena inicial
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pluginActivity = new AndroidJavaObject("com.randomadjective.uactivity.PluginActivity");
        //maxControls = panelManager.panels.Count; // Asignar el número de controles según la cantidad de paneles
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
            /*var message = messageToSend.text;
            Debug.Log("Message to send: " + message);
            _pluginActivity.Call("sendMessageToSmartwatch", message);*/
        }
    }

    /*public void OnMessageReceived(string message)
    {
        Debug.Log("Mensaje recibido en Unity: " + message);
        messageReceived.text = "Mensaje recibido: " + message;
        panelManager.HandleIncomingMessage(message);
    }*/

    public void OnMessageReceived(string message)
    {
        //Debug.Log("Mensaje recibido en Unity: " + message);
        //messageReceived.text = "Mensaje recibido: " + message;

        // Buscar el controlador de la escena actual
        //var controller = GameObject.FindObjectOfType<IGameController>();
        GameObject controllerObject = GameObject.Find("GameController");
        var controller = controllerObject?.GetComponent<IGameController>();
        controller?.HandleMessage(message);
    }

    /*public void NextControl()
    {
        if (_pluginActivity != null)
        {
            currentControlIndex++;
            if (currentControlIndex > maxControls) currentControlIndex = 1;
            UpdateControl();

        }
    }

    public void PreviousControl()
    {
        if (_pluginActivity != null)
        {
            currentControlIndex--;
            if (currentControlIndex < 1) currentControlIndex = maxControls;
            UpdateControl();
        }
    }*/

    private void UpdateControl()
    {
        string sceneName = $"MiniGame_{currentControlIndex}";
        Debug.Log("Cargando escena: " + sceneName);

        // Enviar al smartwatch
        if (_pluginActivity != null)
            _pluginActivity.Call("sendMessageToSmartwatch", $"control_{currentControlIndex}");

        // Cargar escena del minijuego
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

}
