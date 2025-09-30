using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EmulatorButton
{
    public string label;
    public string onClickMessage;
    public string onHoldMessage;
    public string onReleaseMessage;
}

public class PluginEmulatorWindow : EditorWindow
{
    [SerializeField]
    private List<EmulatorButton> buttons = new List<EmulatorButton>()
    {
        new EmulatorButton()
        {
            label = "Up",
            onClickMessage = "Joystick:0,1",
            onHoldMessage = "Joystick:0,1",
            onReleaseMessage = "JoystickRelease:0,1"
        },

        new EmulatorButton()
        {
            label = "Right",
            onClickMessage = "Joystick:1,0",
            onHoldMessage = "Joystick:1,0",
            onReleaseMessage = "JoystickRelease:1,0"
        },

        new EmulatorButton()
        {
            label = "Down",
            onClickMessage = "Joystick:0,-1",
            onHoldMessage = "Joystick:0,-1",
            onReleaseMessage = "JoystickRelease:0,-1"
        },

        new EmulatorButton()
        {
            label = "Left",
            onClickMessage = "Joystick:-1,0",
            onHoldMessage = "Joystick:-1,0",
            onReleaseMessage = "JoystickRelease:-1,0"
        },

         new EmulatorButton()
        {
            label = "Tap",
            onClickMessage = "Tap",
            onHoldMessage = string.Empty,
            onReleaseMessage = string.Empty
        },
    };

    private string customMessage = "";
    private bool isHolding = false;
    private int holdingButtonIndex = -1;

    [MenuItem("Tools/Plugin Activity/Emulator")]
    public static void ShowWindow()
    {
        GetWindow<PluginEmulatorWindow>("Plugin Emulator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Plugin Emulator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Draw all configurable buttons
        for (int i = 0; i < buttons.Count; i++)
        {
            var btn = buttons[i];
            Rect rect = GUILayoutUtility.GetRect(100, 30);

            bool isThisHeld = (isHolding && holdingButtonIndex == i);
            GUIStyle style = new GUIStyle(EditorStyles.miniButton);

            if (isThisHeld)
                style.normal = style.active;
            
            GUI.Box(rect, btn.label, style);

            Event e = Event.current;
            if (rect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    SendMessage(btn.onClickMessage);
                    isHolding = true;
                    holdingButtonIndex = i;
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
                {
                    if (holdingButtonIndex == i)
                    {
                        SendMessage(btn.onReleaseMessage);
                        isHolding = false;
                        holdingButtonIndex = -1;
                        e.Use();
                    }
                }
            }
        }

        // If holding, repeatedly send hold message
        if (isHolding && holdingButtonIndex >= 0 && holdingButtonIndex < buttons.Count)
        {
            SendMessage(buttons[holdingButtonIndex].onHoldMessage);
            Repaint(); // force redraw every frame while holding
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Message", EditorStyles.boldLabel);

        customMessage = EditorGUILayout.TextField("Message", customMessage);
        if (GUILayout.Button("Send", GUILayout.Height(25)))
        {
            if (!string.IsNullOrEmpty(customMessage))
            {
                SendMessage(customMessage);
                customMessage = "";
            }
        }
    }

    private void SendMessage(string message)
    {
        Debug.Log(message);

        if (!Application.isPlaying) return;
        if (string.IsNullOrEmpty(message)) return;

        var controllers = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var controller in controllers)
        {
            if (controller is IGameController gameController)
            {
                gameController.HandleMessage(message);
            }
        }
    }
}
