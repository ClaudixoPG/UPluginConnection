using UnityEngine;


namespace GolfGame
{
    public class ArrowFill : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform arrowTransform;
        [SerializeField] private float maxScaleY = 1f;
        [SerializeField] private float minScaleY = 0.5f;

        private Vector3 baseScale;

        void Awake()
        {
            if (!arrowTransform)
            {
                arrowTransform = transform;
                baseScale = arrowTransform.localScale;
            }
        }

        private void Update()
        {
            
        }
        public void SetFill(float launchForce)
        {
            launchForce = Mathf.Clamp01(launchForce);
            float y = Mathf.Lerp(minScaleY, maxScaleY, launchForce);
            var updateScale = baseScale;
            updateScale.y = y;
            arrowTransform.localScale = updateScale;    
        }



    }
}

