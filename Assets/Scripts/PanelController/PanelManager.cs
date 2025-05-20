using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public List<PanelController> panels;
    private PanelController currentPanel;

    private void Start()
    {
        SetActivePanel(0);
    }

    public void SetActivePanel(int index)
    {
        if (currentPanel != null) currentPanel.Hide();
        currentPanel = panels[index];
        currentPanel.Show();
    }

    public void HandleIncomingMessage(string message)
    {
        currentPanel?.HandleMessage(message);
    }
}
