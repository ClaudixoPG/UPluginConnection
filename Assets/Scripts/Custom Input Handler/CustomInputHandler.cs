using UnityEngine;
using UnityEngine.InputSystem;

public class CustomInputHandler : MonoBehaviour, IGameController
{
    private PlayerInputActions inputActions;
    
    public delegate void ReceiveKey(KeyCode key);
    public static event ReceiveKey onReceiveKey;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.World.Enable();

        BindActions();
    }

    private void BindActions()
    {
        inputActions.World.Tap.performed += ctx =>
        {
            onReceiveKey?.Invoke(KeyCode.KeypadEnter);
        };
    }

    public void HandleMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        // --- Tap ---
        if (message.StartsWith("Tap"))
        {
            onReceiveKey?.Invoke(KeyCode.KeypadEnter);   
            return;
        }

        // --- Button:A / B / X / Y ---
        if (message.StartsWith("Button:"))
        {
            string button = message.Substring("Button:".Length).ToUpper();
            switch (button)
            {
                case "A":
                    onReceiveKey?.Invoke(KeyCode.A);
                    onReceiveKey?.Invoke(KeyCode.UpArrow);
                    break;
                case "B":
                    onReceiveKey?.Invoke(KeyCode.B);
                    onReceiveKey?.Invoke(KeyCode.RightArrow);
                    break;
                case "C":
                    onReceiveKey?.Invoke(KeyCode.C);
                    onReceiveKey?.Invoke(KeyCode.DownArrow);
                    break;
                case "D":
                    onReceiveKey?.Invoke(KeyCode.D);
                    onReceiveKey?.Invoke(KeyCode.LeftArrow);
                    break;
            }
            return;
        }
    }
}
