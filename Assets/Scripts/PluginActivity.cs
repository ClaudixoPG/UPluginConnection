using UnityEngine;

public class PluginActivity : MonoBehaviour
{
    AndroidJavaObject _pluginActivity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pluginActivity = new AndroidJavaObject("com.ra.uactivity.PluginActivity");
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
