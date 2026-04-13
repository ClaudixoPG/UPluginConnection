using UnityEngine;

namespace RythmGame
{
    public class Note : MonoBehaviour
    {
        public bool wasHit = false;
        public int scorePerNote = 100;
        public int scorePerGoodNote = 125;
        public int scorePerPerfectNote = 150;

        public void Hit(ButtonController.State state)
        {
            if (wasHit) return;

            wasHit = true;
            int value = 0;

            switch (state)
            {
                case ButtonController.State.Hit:
                    GameController.instance.hitNote++;
                    value = scorePerNote;
                    break;

                case ButtonController.State.Good:
                    GameController.instance.goodNote++;
                    value = scorePerGoodNote;
                    break;

                case ButtonController.State.Perfect:
                    GameController.instance.perfectNote++;
                    value = scorePerPerfectNote;
                    break;

                default:
                    value = 0;
                    break;
            }

            GameController.instance.NoteHit(value);
            Destroy(gameObject);
        }

        public void Missed()
        {
            if (wasHit) return;

            GameController.instance.missedNote++;
            GameController.instance.NoteMissed();
            Destroy(gameObject);
        }
    }
}