using System.Collections.Generic;
using UnityEngine;

namespace RythmGame
{
    public class NoteSequenceSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class LaneDefinition
        {
            public string laneName;
            public ButtonController button;
            public float zRotation;
        }

        [Header("References")]
        [SerializeField] private Transform noteContainer;
        [SerializeField] private Note notePrefab;
        [SerializeField] private List<LaneDefinition> lanes = new List<LaneDefinition>();

        [Header("Sequence Timing")]
        [SerializeField] private float initialLeadInSeconds = 2f;

        [SerializeField] private bool useBeatScrollerTiming = true;
        [SerializeField] private float beatsPerNote = 1f;

        [SerializeField] private float noteIntervalSeconds = 0.75f;

        [Header("Rules")]
        [SerializeField] private bool avoidSameLaneTwiceInARow = true;

        public float NoteIntervalSeconds => noteIntervalSeconds;

        public int GenerateSequence(float songDurationSeconds, float scrollSpeedUnitsPerSecond, BeatScroller beatScroller = null)
        {
            ClearNotes();

            if (noteContainer == null || notePrefab == null || lanes.Count == 0)
            {
                Debug.LogWarning("[NoteSequenceSpawner] Missing references.");
                return 0;
            }

            float resolvedNoteInterval = ResolveNoteInterval(beatScroller);

            int notesToSpawn = Mathf.Max(
                1,
                Mathf.FloorToInt((songDurationSeconds - initialLeadInSeconds) / resolvedNoteInterval)
            );

            int previousLaneIndex = -1;
            float targetHitY = GetAverageButtonY();

            for (int i = 0; i < notesToSpawn; i++)
            {
                int laneIndex = GetLaneIndex(previousLaneIndex);
                var lane = lanes[laneIndex];

                float hitTime = initialLeadInSeconds + (i * resolvedNoteInterval);
                float worldY = targetHitY + (scrollSpeedUnitsPerSecond * hitTime);
                float worldX = lane.button.transform.position.x;

                Vector3 worldPos = new Vector3(worldX, worldY, 0f);
                Vector3 localPos = noteContainer.InverseTransformPoint(worldPos);

                Note note = Instantiate(notePrefab, noteContainer);
                note.transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);
                note.transform.localRotation = Quaternion.Euler(0f, 0f, lane.zRotation);

                previousLaneIndex = laneIndex;
            }

            return notesToSpawn;
        }

        public void ClearNotes()
        {
            if (noteContainer == null) return;

            var toDestroy = new List<GameObject>();

            for (int i = 0; i < noteContainer.childCount; i++)
            {
                toDestroy.Add(noteContainer.GetChild(i).gameObject);
            }

            for (int i = 0; i < toDestroy.Count; i++)
            {
                Destroy(toDestroy[i]);
            }
        }

        private float ResolveNoteInterval(BeatScroller beatScroller)
        {
            if (useBeatScrollerTiming && beatScroller != null)
            {
                return beatScroller.SecondsPerBeat * beatsPerNote;
            }

            return noteIntervalSeconds;
        }

        private float GetAverageButtonY()
        {
            float total = 0f;
            int count = 0;

            for (int i = 0; i < lanes.Count; i++)
            {
                if (lanes[i].button == null) continue;
                total += lanes[i].button.transform.position.y;
                count++;
            }

            if (count == 0) return 0f;
            return total / count;
        }

        private int GetLaneIndex(int previousLaneIndex)
        {
            if (!avoidSameLaneTwiceInARow || lanes.Count <= 1)
                return Random.Range(0, lanes.Count);

            int index = Random.Range(0, lanes.Count);
            int safety = 0;

            while (index == previousLaneIndex && safety < 20)
            {
                index = Random.Range(0, lanes.Count);
                safety++;
            }

            return index;
        }
    }
}