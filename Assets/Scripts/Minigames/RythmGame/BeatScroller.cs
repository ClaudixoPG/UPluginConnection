using UnityEngine;

namespace RythmGame
{
    public class BeatScroller : MonoBehaviour
    {
        public float beatTempo; // Beats per minute
        public bool hasStarted; // Has the song started?

        private void Start()
        {
            beatTempo = beatTempo / 60f; // Convert to beats per second
        }

        void Update()
        {
            if (hasStarted)
            {
                transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
            }
        }

    }
}