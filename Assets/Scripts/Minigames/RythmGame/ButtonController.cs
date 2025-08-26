using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace RythmGame
{
    public class ButtonController : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        public Sprite defaultSprite;
        public Sprite pressedSprite;

        // Start is called once before the first execution of Update after the MonoBehaviour is created

        List<Note> notes = new List<Note>();

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
                //notes[0].Hit();
                //notes.RemoveAt(0);
                var note = notes[0];
                notes.RemoveAt(0);
                note.Hit();
            }
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
                }
            }
        }
    }
}
