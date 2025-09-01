using UnityEngine;

namespace FishingGame
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Transform topPivot;
        [SerializeField] Transform bottomPivot;
        [SerializeField] Transform fish;

        float fishPosition;
        float fishDestination;
        float fishTimer;

        [SerializeField] float timerMultiplicator = 3f;
        float fishSpeed;
        [SerializeField] float smoothMotion = 1f;

        [SerializeField] Transform hook;
        float hookPosition;
        [SerializeField] float hookSize = 12f;
        [SerializeField] float hookPower = 5f;
        float hookProgress;
        float hookPullVelocity;
        [SerializeField] float hookPullPower = 0.01f;
        [SerializeField] float hookGravityPower = 0.005f;
        [SerializeField] float hookProgressDegradationPower = 0.1f;

        [SerializeField] Transform progressBarContainer;

        [SerializeField] float failTimer = 10f;
        bool isReeling = false;

        void Start()
        {
            hookSize = ComputeHookSizeNormalized(); // ahora sí, hookSize vuelve a ser 0..1
        }

        float ComputeHookSizeNormalized()
        {
            float railHeight = Vector3.Distance(bottomPivot.position, topPivot.position);

            // SpriteRenderer
            var sr = hook.GetComponent<SpriteRenderer>();
            if (sr != null)
                return Mathf.Clamp01(sr.bounds.size.y / railHeight);

            // UI (RectTransform)
            var rt = hook.GetComponent<RectTransform>();
            if (rt != null)
                return Mathf.Clamp01((rt.rect.height * hook.lossyScale.y) / railHeight);

            // Fallback (menos preciso)
            return Mathf.Clamp01(hook.lossyScale.y / railHeight);
        }

        void Update()
        {
            Fish();
            HookSequence();
            ProgressCheck();
        }

        void ProgressCheck()
        {
            Vector3 ls = progressBarContainer.localScale;
            ls.y = hookProgress;
            progressBarContainer.localScale = ls;

            float min = hookPosition;
            float max = hookPosition + hookSize;

            if (min < fishPosition && fishPosition < max)
            {
                hookProgress += hookPower * Time.deltaTime;
            }
            else
            {
                hookProgress -= hookProgressDegradationPower * Time.deltaTime;
                failTimer -= failTimer * Time.deltaTime;
                if (failTimer < 0)
                {
                    Lose();
                }
            }

            if (hookProgress >= 1f)
            {
                Win();
            }
            hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
        }

        public void HookInput()
        {
            isReeling = true;
            
        }

        public void CancelHookInput()
        {
            isReeling = false;
        }

        void HookSequence()
        {
            if (isReeling) // ------------------------------------------------------------ Cambiar input
            {
                hookPullVelocity += hookPullPower * Time.deltaTime;
            }
            hookPullVelocity -= hookGravityPower * Time.deltaTime;

            hookPosition += hookPullVelocity;

            if (hookPosition <= 0f && hookPullVelocity < 0f) // Está abajo intentando bajar
            {
                hookPullVelocity = 0f;
            }
            if (hookPosition + hookSize >= 1f && hookPullVelocity > 0f) // Está arriba intentando subir
            {
                hookPullVelocity = 0f;
            }

            hookPosition = Mathf.Clamp(hookPosition, 0, topPivot.position.y - hookSize);
            hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
        }

        void Fish()
        {
            fishTimer -= Time.deltaTime;
            if (fishTimer < 0f)
            {
                fishTimer = UnityEngine.Random.value * timerMultiplicator;

                fishDestination = UnityEngine.Random.value;
            }

            fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
            fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
        }

        void Win()
        {

        }

        void Lose()
        {

        }
    }

}