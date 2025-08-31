using UnityEngine;
using UnityEngine.Rendering;

public class FishScript : MonoBehaviour
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
    [SerializeField] float hookSize = 0.1f;
    [SerializeField] float hookPower = 5f;
    float hookProgress;
    float hookPullVelocity;
    [SerializeField] float hookPullPower = 0.01f;
    [SerializeField] float hookGravityPower = 0.005f;
    [SerializeField] float hookProgressDegradationPower = 0.1f;

    [SerializeField] Transform progressBarContainer;

    [SerializeField] float failTimer = 10f;

    void Start()
    {

    }
    void Update()
    {
        Fish();
        Hook();
        ProgressCheck();
    }

    void ProgressCheck()
    {
        Vector3 ls = progressBarContainer.localScale;
        ls.y = hookProgress;
        progressBarContainer.localScale = ls;

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if(min < fishPosition && fishPosition < max)
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

        if(hookProgress >= 1f)
        {
            Win();
        }
        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }

    void Hook()
    {
        if(Input.GetMouseButton(0)) // ------------------------------------------------------------ Cambiar input
        {
            hookPullVelocity += hookPullPower * Time.deltaTime;
        }
        hookPullVelocity -= hookGravityPower * Time.deltaTime;

        hookPosition += hookPullVelocity;

        if(hookPosition - hookSize / 2 <= 0f && hookPullVelocity < 0f) // Está abajo intentando bajar
        {
            hookPullVelocity = 0f;
        }
        if(hookPosition + hookSize / 2 >= 1f && hookPullVelocity > 0f) // Está arriba intentando subir
        {
            hookPullVelocity = 0f;
        }

        hookPosition = Mathf.Clamp(hookPosition, hookSize / 2, 1 - hookSize / 2);
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
