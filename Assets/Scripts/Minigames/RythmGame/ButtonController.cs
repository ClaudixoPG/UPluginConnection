using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
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
        public Sprite defaultSprite;
        public Sprite pressedSprite;

        List<Note> notes = new List<Note>();
        State state = State.Miss;

        //Effects
        public List<GameObject> effects = new List<GameObject>();

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = defaultSprite;
        }

        public void PressButton()
        {
            spriteRenderer.sprite = pressedSprite;

            if(notes.Count > 0)
            {
                var note = notes[0];
                notes.RemoveAt(0);
                note.Hit(CheckNoteState(note));
            }
        }

        State CheckNoteState(Note note)
        {
            float yDifference = Mathf.Abs(note.transform.position.y - transform.position.y);
            if (yDifference > 0.25f)
            {
                state = State.Hit;
                Instantiate(effects[0], note.transform.position, effects[0].transform.rotation);
            }
            else if (yDifference > 0.05f)
            {
                state = State.Good;
                Instantiate(effects[1], note.transform.position, effects[1].transform.rotation);
            }
            else
            {
                state = State.Perfect;
                Instantiate(effects[2], note.transform.position, effects[2].transform.rotation);
            }
            return state;
        }
        public void ReleaseButton()
        {
            spriteRenderer.sprite = defaultSprite;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Note>(out var note))
            {
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
                    Instantiate(effects[3], note.transform.position, effects[0].transform.rotation);
                }
            }
        }
    }
}
