using UnityEngine;
using static PrologueSceneManager;

public class PrologueSceneManager : MonoBehaviour
{
    public delegate void PrologueSceneEnds();
    public static event PrologueSceneEnds onPrologueSceneEnds;

    public void Event_AnimationEnds()
    {
        onPrologueSceneEnds?.Invoke();
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Prologue");
    }
}
