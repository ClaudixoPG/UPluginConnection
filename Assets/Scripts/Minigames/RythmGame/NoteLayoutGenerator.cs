using System.Collections.Generic;
using UnityEngine;

namespace RythmGame
{
    [ExecuteAlways]
    public class NoteLayoutGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class LaneConfig
        {
            public string laneName;
            public float xPosition;
            public float zRotation;
        }

        [Header("Lane Setup")]
        [SerializeField] private List<LaneConfig> lanes = new List<LaneConfig>();

        [Header("Vertical Layout")]
        [SerializeField] private float startY = 3.5f;
        [SerializeField] private float stepY = 1.0f;

        [Header("Rules")]
        [SerializeField] private bool avoidSameLaneTwiceInARow = true;

        [ContextMenu("Generate Note Layout")]
        public void GenerateLayout()
        {
            if (lanes == null || lanes.Count == 0)
            {
                Debug.LogWarning("[NoteLayoutGenerator] No lanes configured.");
                return;
            }

            var notes = new List<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                if (child.GetComponent<Note>() != null)
                {
                    notes.Add(child);
                }
            }

            if (notes.Count == 0)
            {
                Debug.LogWarning("[NoteLayoutGenerator] No Note children found.");
                return;
            }

            int previousLaneIndex = -1;

            for (int i = 0; i < notes.Count; i++)
            {
                int laneIndex = GetRandomLaneIndex(previousLaneIndex);
                var lane = lanes[laneIndex];

                Vector3 pos = notes[i].localPosition;
                pos.x = lane.xPosition;
                pos.y = startY - (i * stepY);
                pos.z = 0f;

                notes[i].localPosition = pos;
                notes[i].localRotation = Quaternion.Euler(0f, 0f, lane.zRotation);

                previousLaneIndex = laneIndex;
            }
        }

        private int GetRandomLaneIndex(int previousLaneIndex)
        {
            if (!avoidSameLaneTwiceInARow || lanes.Count <= 1)
            {
                return Random.Range(0, lanes.Count);
            }

            int laneIndex = Random.Range(0, lanes.Count);

            int safety = 0;
            while (laneIndex == previousLaneIndex && safety < 20)
            {
                laneIndex = Random.Range(0, lanes.Count);
                safety++;
            }

            return laneIndex;
        }
    }
}