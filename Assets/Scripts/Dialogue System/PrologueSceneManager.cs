using UnityEngine;
using UnityEngine.InputSystem;
using static PrologueSceneManager;

public class PrologueSceneManager : MonoBehaviour
{
    private PlayerInputActions _actions;

    public delegate void PrologueSceneEnds();
    public static event PrologueSceneEnds onPrologueSceneEnds;

    private void Start()
    {
        _actions = new PlayerInputActions();
        _actions.World.Enable();
    }

    private void Update()
    {
        if (_actions.World.Enter.WasPressedThisFrame())
        {
            _actions.World.Disable(); 
            Event_AnimationEnds();
        }
    }

    public void Event_AnimationEnds()
    {
        onPrologueSceneEnds?.Invoke();
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Prologue");
    }
}
