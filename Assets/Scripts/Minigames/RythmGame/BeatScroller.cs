using UnityEngine;

namespace RythmGame
{
    public class BeatScroller : MonoBehaviour
    {
        [SerializeField] private float beatTempo = 120f;

        private Vector3 initialPosition;
        private float beatsPerSecond;

        public bool hasStarted { get; private set; }
        public float ScrollSpeedUnitsPerSecond => beatsPerSecond;
        public float BeatTempo => beatTempo;
        public float SecondsPerBeat => 60f / beatTempo;

        private void Awake()
        {
            initialPosition = transform.position;
        }

        private void Start()
        {
            beatsPerSecond = beatTempo / 60f;
        }

        private void Update()
        {
            if (!hasStarted) return;

            transform.position -= new Vector3(0f, beatsPerSecond * Time.deltaTime, 0f);
        }

        public void SetStarted(bool value)
        {
            hasStarted = value;
        }

        public void ResetScroller()
        {
            hasStarted = false;
            transform.position = initialPosition;
        }
    }
}