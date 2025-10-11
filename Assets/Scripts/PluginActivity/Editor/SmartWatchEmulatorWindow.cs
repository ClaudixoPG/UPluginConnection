using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class EmulatorButton
{
    public string label;
    public string onClickMessage;
    public string onHoldMessage;
    public string onReleaseMessage;
}

public class SmartWatchEmulatorWindow : EditorWindow
{
    private enum InputType
    {
        NONE, BEGIN, HOLD, RELEASE
    }

    [SerializeField]
    private List<EmulatorButton> digitalButtons = new List<EmulatorButton>()
    {
        new EmulatorButton()
        {
            label = "Tap",
            onClickMessage = "Tap",
            onHoldMessage = "Tap_hold",
            onReleaseMessage = "Tap_release"
        },
    };
    private List<EmulatorButton> dpadButtons = new List<EmulatorButton>()
    {
        new EmulatorButton()
        {
            label = "Up",
            onClickMessage = "Dpad:0,1",
            onHoldMessage = "Dpad:0,1",
            onReleaseMessage = "DpadRelease:0,1"
        },

        new EmulatorButton()
        {
            label = "Right",
            onClickMessage = "Dpad:1,0",
            onHoldMessage = "Dpad:1,0",
            onReleaseMessage = "DpadRelease:1,0"
        },

        new EmulatorButton()
        {
            label = "Down",
            onClickMessage = "Dpad:0,-1",
            onHoldMessage = "Dpad:0,-1",
            onReleaseMessage = "DpadRelease:0,-1"
        },

        new EmulatorButton()
        {
            label = "Left",
            onClickMessage = "Dpad:-1,0",
            onHoldMessage = "Dpad:-1,0",
            onReleaseMessage = "DpadRelease:-1,0"
        },
    };

    private bool _tabMode = true;

    //JoystickMode
    private InputType _joystickMode = InputType.NONE;
    private bool isNormalized;
    private int currentRange = 10;

    //SensorMode
    private InputType _sensorMode = InputType.NONE;
    private float _sensorTimer = 1;
    private float _sensorScale = 1;
    private float _sensorBarValue = 0;
    private bool _slowReset;
    private float _slowResetValue = 1;
    private bool _isPressed = false; // flag persistente entre frames
    double lastTime;

    private string customMessage = "";
    private bool isHolding = false;

    private int holdingButtonIndex = -1;

    private int _tab;


    [MenuItem("Tools/SmartWatch Emulator")]
    public static void ShowWindow()
    {
        GetWindow<SmartWatchEmulatorWindow>("Smartwatch Emulator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Smartwatch Emulator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _tabMode = EditorGUILayout.Toggle("Tab Display", _tabMode);

        EditorGUILayout.Space();

        string[] tabs = new string[] { "Buttons", "Joystick", "D-Pad", "Sensors" };

        if (_tabMode)
        {
            _tab = GUILayout.Toolbar(_tab, tabs);
        }
        else
        {
            _tab = EditorGUILayout.Popup("Mode", _tab, tabs);
        }
        
        EditorGUILayout.Space(50);

        switch (_tab)
        {
            case 0:
                DrawButtons(digitalButtons.ToArray());
                break;
            case 1:
                _joystickMode = PaintJoystickMode(_joystickMode, isNormalized, currentRange, out int range, out bool normalized, out Vector2 coords);

                currentRange = range;
                isNormalized = normalized;

                switch (_joystickMode)
                {
                    case InputType.BEGIN:
                        SendMessage($"Joystick:{coords.x},{coords.y}");
                        break;
                    case InputType.HOLD:
                        SendMessage($"Joystick:{coords.x},{coords.y}");
                        break;
                    case InputType.RELEASE:
                        SendMessage($"JoystickRelease:{coords.x},{coords.y}");

                        _joystickMode = InputType.NONE;
                        break;
                }

                Repaint();

                break;
            case 2:
                DrawButtons(dpadButtons.ToArray());
                break;
            case 3:

                // Actualiza el estado del sensor
                _sensorMode = PaintSensors(_sensorMode, _sensorTimer, _sensorScale, _sensorBarValue, out float timer, out float scale, out float value);

                _sensorTimer = timer;
                _sensorScale = scale;

                if (_sensorMode == InputType.RELEASE)
                {
                    _sensorBarValue = 0;
                    SendMessage($"Time:{value}");
                    _joystickMode = InputType.NONE;
                }
                else
                {
                    _sensorBarValue = value;
                }

                // Fuerza el repintado para que la barra se llene en tiempo real
                Repaint();
                break;
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



    private InputType PaintSensors(InputType current, float currentTimer, float currentScale, float currentValue, out float timer, out float scale, out float value)
    {
        // Copiamos valores actuales
        timer = Mathf.Max(0.01f, currentTimer); // evitar cero o negativos
        scale = currentScale;
        value = currentValue;

        // === PRIMERA FILA ===
        GUILayout.BeginHorizontal();

        // Campo de timer
        timer = EditorGUILayout.FloatField(timer, GUILayout.Width(150));

        // Barra de progreso
        float progress = Mathf.Clamp01(value / Mathf.Max(timer, 0.0001f));
        Rect progressRect = GUILayoutUtility.GetRect(100, 18);
        EditorGUI.ProgressBar(progressRect, progress, $"{(progress * 100f):F0}%");

        // Tooltip dinámico sobre la barra
        if (Event.current.type == EventType.Repaint)
        {
            float tooltipValue = value * scale;
            string tooltipText = $"{tooltipValue:F2}";
            Vector2 tooltipSize = GUI.skin.label.CalcSize(new GUIContent(tooltipText));

            float x = Mathf.Lerp(progressRect.xMin, progressRect.xMax - tooltipSize.x - 6f, progress);
            float y = progressRect.yMin - tooltipSize.y - 4f;
            Rect tooltipRect = new Rect(x, y, tooltipSize.x + 6f, tooltipSize.y + 4f);

            EditorGUI.DrawRect(tooltipRect, new Color(0, 0, 0, 0.75f));
            GUI.Label(new Rect(tooltipRect.x + 3, tooltipRect.y + 2, tooltipSize.x, tooltipSize.y), tooltipText);
        }

        GUILayout.Label("X", GUILayout.Width(20));

        // Campo de scale
        scale = EditorGUILayout.FloatField(scale, GUILayout.Width(80));

        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        Rect buttonRect = GUILayoutUtility.GetRect(80, 25);

        Event e = Event.current;

        if (!_isPressed)
        {
            if (e.rawType == EventType.MouseDown && buttonRect.Contains(e.mousePosition))
            {
                _isPressed = true;
                lastTime = EditorApplication.timeSinceStartup;
                e.Use();
            }
        }
        else
        {
            if (e.rawType == EventType.MouseUp)
            {
                _isPressed = false;
                e.Use();
            }
        }

        GUI.Button(buttonRect, "Press");


        // === LÓGICA DE ESTADOS usando _isPressed ===
        switch (current)
        {
            case InputType.NONE:
                if (_isPressed)
                {
                    // Comenzar la barra
                    value = 0f;
                    return InputType.HOLD;
                }
                break;

            case InputType.HOLD:
                if (_isPressed)
                {
                    // Incrementar valor 
                    double currentTime = EditorApplication.timeSinceStartup;
                    float deltaTime = (float)(currentTime - lastTime);
                    lastTime = currentTime;

                    value += (float)deltaTime;
                    if (value > timer) value = timer;
                }
                else
                {
                    // Se soltó el botón
                    value *= scale;
                    return InputType.RELEASE;
                }
                break;

            case InputType.RELEASE:
                if (!_isPressed)
                {
                    // Reiniciar
                    value = 0f;
                    return InputType.NONE;
                }
                break;
        }

        return current;
    }

    private void DrawButtons(EmulatorButton[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
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
        if (isHolding && holdingButtonIndex >= 0 && holdingButtonIndex < buttons.Length)
        {
            SendMessage(buttons[holdingButtonIndex].onHoldMessage);
            Repaint(); // force redraw every frame while holding
        }
    }

    private InputType PaintJoystickMode(InputType current, bool currentNormalizedMode, int currentRange, out int range, out bool isNormalized, out Vector2 coords)
    {
        const float rectSize = 300;
        coords = Vector2.zero;

        range = currentRange;
        isNormalized = currentNormalizedMode;

        // Espaciado
        GUILayout.Space(10);
        Rect rect = GUILayoutUtility.GetRect(rectSize, rectSize, GUILayout.ExpandWidth(true));
        Event e = Event.current;

        // Centro del rectángulo
        Vector2 center = rect.center;

        float horizontalMargin = 100f; // margen lateral
        float verticalMargin = 10f;   // margen superior e inferior opcional
        rect.xMin += horizontalMargin;
        rect.xMax -= horizontalMargin;
        rect.yMin += verticalMargin;
        rect.yMax -= verticalMargin;

        // Dibuja fondo
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 1f));

        // Dibuja ejes
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(center.x, rect.yMin), new Vector2(center.x, rect.yMax));
        Handles.DrawLine(new Vector2(rect.xMin, center.y), new Vector2(rect.xMax, center.y));

        // Toggle de normalización
        isNormalized = GUILayout.Toggle(isNormalized, "Normalized");

        if(!isNormalized)
            range = EditorGUILayout.IntField("Range", range);

        if (range < 1)
            range = 1;

        // Detectar mouse
        Vector2 mousePos = e.mousePosition;
        bool inside = rect.Contains(mousePos);

        if (inside)
        {
            Vector2 pixel = mousePos - center;
            pixel.y *= -1f;

            if (isNormalized)
            {
                // Usa el mismo radio que dibujas: aquí uso el 0.25f que tenías; cambia si quieres otro tamaño.
                float circleRadius = Mathf.Min(rect.width, rect.height) * 0.25f;

                // Normalizamos por el radio del círculo (mismo factor X/Y)
                Vector2 normalized = pixel / circleRadius;

                // Si está fuera del círculo lo capamos al borde
                if (normalized.magnitude > 1f)
                    normalized = normalized.normalized;

                coords = normalized; // coords en -1..1 dentro del círculo

                // Dibuja el círculo con el mismo radio
                Handles.color = new Color(0.2f, 0.6f, 1f, 0.4f);
                Handles.DrawSolidDisc(center, Vector3.forward, circleRadius);
            }
            else
            {
                // modo no normalizado: coords en rango [-10, 10]
                // Mapea la posición del mouse (pixel.x / (rect.width/2)) al rango -10 a 10
                coords.x = Mathf.Clamp((pixel.x / (rect.width / 2f)) * range, -range, range);
                coords.y = Mathf.Clamp((pixel.y / (rect.height / 2f)) * range, -range, range);
            }

            string tooltip = $"({coords.x:F2}, {coords.y:F2})";
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(tooltip));
            Rect tooltipRect = new Rect(mousePos.x + 15, mousePos.y - size.y / 2, size.x + 8, size.y + 4);
            EditorGUI.DrawRect(tooltipRect, new Color(0, 0, 0, 0.7f));
            GUI.Label(new Rect(tooltipRect.x + 4, tooltipRect.y + 2, size.x, size.y), tooltip);
        }

        // Control de InputType
        switch (e.type)
        {
            case EventType.MouseDown:
                if (inside && e.button == 0 && current == InputType.NONE)
                {
                    GUI.FocusControl(null);
                    e.Use();
                    return InputType.BEGIN;
                }
                break;

            case EventType.MouseDrag:
                if (current == InputType.BEGIN)
                {
                    e.Use();
                    return InputType.HOLD;
                }
                break;

            case EventType.MouseUp:
                if ((current == InputType.BEGIN || current == InputType.HOLD) && e.button == 0)
                {
                    e.Use();
                    return InputType.RELEASE;
                }
                break;
        }

        return current;
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
