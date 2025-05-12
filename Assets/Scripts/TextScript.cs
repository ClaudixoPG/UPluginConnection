using TMPro;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public TextMeshProUGUI text;

    AndroidJavaObject _pluginActivity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = "";
        appendText("Begin method start of TextScipt");

        _pluginActivity = new AndroidJavaObject("com.ra.uactivity.PluginActivity");

        if ( _pluginActivity != null )
        {
            string str = _pluginActivity.Get<string>("myString"); //obtengo la variable de la clase Java creada en Android Native
            appendText("My String: " + str);
            appendText("Change my string...");

            _pluginActivity.Set<string>("myString", "Hello from Unity!"); //cambio la variable de la clase Java creada en Android Native
            str = _pluginActivity.Call<string>("getMyString"); //llamo al método getMyString

            appendText("My String changed: " + str);
            string vals = getStringFromArray(_pluginActivity.Call<float[]>("getMyValues")); //llamo al método getMyArray

            appendText("My values: " + vals);
            appendText("Changing my values adding 5.1...");
            _pluginActivity.Call("ChangeMyValues", 5.1f);
            vals = getStringFromArray(_pluginActivity.Call<float[]>("getMyValues"));
            appendText("My changed values: " + vals);

            Invoke("invokedmethod", 5);
        }

        appendText("End method start of TextScipt\n");

    }


    //Methods and auxiliars

    string getStringFromArray(float[] values)
    {
        string returning = "[";
        for (int it = 0; it < values.Length; it++)
            returning += values[it].ToString() + " ";
        returning += "]";

        return returning;
    }

    void invokedmethod()
    {
        appendText ("Calling Android to notify Unity....");  
        if(_pluginActivity!=null) 
        _pluginActivity.Call("TestSendMessage", transform.name, "appendText"); 
    }

    void appendText(string str)
    {
        string theTime = System.DateTime.Now.ToString("hh:mm:ss: ");
        text.text += theTime + str + "\n";
    }

}
