using RythmGame;
using UnityEngine;

public class Note : MonoBehaviour
{
    public bool wasHit = false;
    public void Hit()
    {
        wasHit = true;
        GameController.instance.NoteHit();
        Destroy(gameObject);
    }

    public void Missed()
    {
        if (wasHit) return;
        GameController.instance.NoteMissed();
        Destroy(gameObject);
    }
}
