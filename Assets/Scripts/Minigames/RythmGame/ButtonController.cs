using System.Collections.Generic;
using UnityEngine;

namespace RythmGame
{
    public class ButtonController : MonoBehaviour
    {
        public enum State
        {
            Hit,
            Good,
            Perfect,
            Miss
        }

        private SpriteRenderer spriteRenderer;

        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite pressedSprite;
        [SerializeField] private List<GameObject> effects = new List<GameObject>();

        private readonly List<Note> notes = new List<Note>();
        private State state = State.Miss;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.sprite = defaultSprite;
        }

        public void PressButton()
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = pressedSprite;

            if (notes.Count == 0) return;

            Note closestNote = GetClosestNote();
            if (closestNote == null) return;

            notes.Remove(closestNote);
            closestNote.Hit(CheckNoteState(closestNote));
        }

        public void ReleaseButton()
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = defaultSprite;
        }

        public void ResetButtonState()
        {
            notes.Clear();

            if (spriteRenderer != null)
                spriteRenderer.sprite = defaultSprite;
        }

        private Note GetClosestNote()
        {
            Note closest = null;
            float closestDistance = float.MaxValue;

            foreach (var note in notes)
            {
                if (note == null) continue;

                float yDifference = Mathf.Abs(note.transform.position.y - transform.position.y);
                if (yDifference < closestDistance)
                {
                    closestDistance = yDifference;
                    closest = note;
                }
            }

            return closest;
        }

        private State CheckNoteState(Note note)
        {
            float yDifference = Mathf.Abs(note.transform.position.y - transform.position.y);

            if (yDifference > 0.25f)
            {
                state = State.Hit;
                if (effects.Count > 0 && effects[0] != null)
                    Instantiate(effects[0], note.transform.position, effects[0].transform.rotation);
            }
            else if (yDifference > 0.05f)
            {
                state = State.Good;
                if (effects.Count > 1 && effects[1] != null)
                    Instantiate(effects[1], note.transform.position, effects[1].transform.rotation);
            }
            else
            {
                state = State.Perfect;
                if (effects.Count > 2 && effects[2] != null)
                    Instantiate(effects[2], note.transform.position, effects[2].transform.rotation);
            }

            return state;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Note>(out var note))
            {
                if (!notes.Contains(note))
                    notes.Add(note);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Note>(out var note))
            {
                notes.Remove(note);

                if (!note.wasHit)
                {
                    note.Missed();
                    state = State.Miss;

                    if (effects.Count > 3 && effects[3] != null)
                        Instantiate(effects[3], note.transform.position, effects[3].transform.rotation);
                }
            }
        }
    }
}