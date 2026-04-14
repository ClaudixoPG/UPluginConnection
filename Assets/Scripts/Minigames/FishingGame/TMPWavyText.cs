using TMPro;
using UnityEngine;

namespace FishingGame
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPWavyText : MonoBehaviour
    {
        [Header("Wave")]
        [SerializeField] private float waveSpeed = 3f;
        [SerializeField] private float waveHeight = 8f;
        [SerializeField] private float characterPhaseOffset = 0.5f;

        [Header("Optional Scale Bounce")]
        [SerializeField] private bool useScaleBounce = false;
        [SerializeField] private float scaleBounceAmount = 0.08f;
        [SerializeField] private float scaleBounceSpeedMultiplier = 1.5f;

        private TextMeshProUGUI textMesh;
        private TMP_TextInfo textInfo;
        private Vector3[][] originalVertices;
        private string lastText = string.Empty;
        private bool isInitialized;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void LateUpdate()
        {
            if (textMesh == null || !textMesh.gameObject.activeInHierarchy)
                return;

            if (!isInitialized || lastText != textMesh.text)
            {
                Initialize();
            }

            AnimateText();
        }

        private void Initialize()
        {
            if (textMesh == null)
                return;

            textMesh.ForceMeshUpdate();
            textInfo = textMesh.textInfo;

            originalVertices = new Vector3[textInfo.meshInfo.Length][];

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                Vector3[] src = textInfo.meshInfo[i].vertices;
                originalVertices[i] = new Vector3[src.Length];
                System.Array.Copy(src, originalVertices[i], src.Length);
            }

            lastText = textMesh.text;
            isInitialized = true;
        }

        private void AnimateText()
        {
            textMesh.ForceMeshUpdate();
            textInfo = textMesh.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int meshIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] sourceVertices = originalVertices[meshIndex];
                Vector3[] destinationVertices = textInfo.meshInfo[meshIndex].vertices;

                Vector3 charMidBaseline = (sourceVertices[vertexIndex] + sourceVertices[vertexIndex + 2]) / 2f;

                float wave = Mathf.Sin(Time.unscaledTime * waveSpeed + i * characterPhaseOffset);
                Vector3 offset = new Vector3(0f, wave * waveHeight, 0f);

                float scale = 1f;
                if (useScaleBounce)
                {
                    float bounce = Mathf.Sin(Time.unscaledTime * waveSpeed * scaleBounceSpeedMultiplier + i * characterPhaseOffset);
                    scale += bounce * scaleBounceAmount;
                }

                for (int j = 0; j < 4; j++)
                {
                    Vector3 orig = sourceVertices[vertexIndex + j];
                    Vector3 relative = orig - charMidBaseline;
                    relative *= scale;
                    destinationVertices[vertexIndex + j] = charMidBaseline + relative + offset;
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }

        public void RefreshNow()
        {
            Initialize();
        }
    }
}