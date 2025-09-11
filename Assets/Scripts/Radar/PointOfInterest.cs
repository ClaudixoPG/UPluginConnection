/*using System.Collections;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public bool IsDetected { get; set; }
    public Renderer rend;
    public Material revealedMaterial;
    public float transitionDuration = 1f;
    public GameObject particles;

    private Material[] uniqueMaterials;

    void Start()
    {
        POIManager.Instance?.RegisterPOI(this);
        if (rend == null) rend = GetComponent<Renderer>();
    }

    public void OnDetected()
    {
        if (IsDetected) return;
        IsDetected = true;

        Debug.Log($"POI detectado: {name}");

        if (rend != null && revealedMaterial != null)
        {
            // Crear copias únicas de los materiales actuales
            Material[] shared = rend.materials;
            uniqueMaterials = new Material[shared.Length];

            for (int i = 0; i < shared.Length; i++)
            {
                uniqueMaterials[i] = new Material(shared[i]); // Copia independiente
            }

            rend.materials = uniqueMaterials;

            // Transición hacia el color base del material revelado
            foreach (var mat in uniqueMaterials)
            {
                if (revealedMaterial.HasProperty("_Color") && mat.HasProperty("_Color"))
                {
                    Color startColor = mat.color;
                    Color targetColor = revealedMaterial.color;

                    StartCoroutine(LerpColor(mat, startColor, targetColor, transitionDuration));
                }
            }
            //añadir partículas si están definidas -> funciona pero no me convencen las partículas
            //if (particles != null)
                //Instantiate(particles, transform.position, Quaternion.identity, transform);
        }
    }

    private IEnumerator LerpColor(Material mat, Color startColor, Color endColor, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            mat.color = Color.Lerp(startColor, endColor, t / duration);
            yield return null;
        }
        mat.color = endColor;
    }
}
*/

using System.Linq;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] private string id;
    public string ID => id;
    public bool IsDetected { get; set; }
    public Renderer rend;
    public Material revealedMaterial;
    public float transitionDuration = 1f;
    public GameObject particles;

    private Material[] uniqueMaterials;
    private LTDescr[] emissionTweens; // para cancelar luego si hace falta

    // Emission
    public bool enableEmissionPulse = true;
    private Color emissionColor = Color.white;
    private float emissionIntensity = 0.5f;
    private float emissionPulseSpeed = .5f;

    private POI_InteractionHandler[] interactionsInThisPOI;

    void Awake()
    {
        interactionsInThisPOI = GetComponents<POI_InteractionHandler>();

        POIManager.Instance?.RegisterPOI(this);
        if (rend == null) rend = GetComponent<Renderer>();
    }

    public void OnUndetected()
    {
        if (!IsDetected) return;
        IsDetected = false;

        Debug.Log($"POI undetected: {name}");

        if (rend != null && uniqueMaterials != null)
        {
            for (int i = 0; i < uniqueMaterials.Length; i++)
            {
                Material mat = uniqueMaterials[i];

                // Cancel emission tweens
                if (emissionTweens != null && emissionTweens[i] != null)
                {
                    LeanTween.cancel(emissionTweens[i].id);
                    emissionTweens[i] = null;
                }

                // Reverse color transition back to original
                Color startColor = mat.color;
                Color endColor = new Color(0.8773585f, 0.2276166f, 0.5634834f, 1); // <-- default (or you can store original color at OnDetected)

                if (revealedMaterial.HasProperty("_Color") && mat.HasProperty("_Color"))
                {
                    LeanTween.value(gameObject, 0f, 1f, transitionDuration)
                        .setOnUpdate((float val) =>
                        {
                            mat.color = Color.Lerp(startColor, endColor, val);
                        })
                        .setEase(LeanTweenType.easeInOutSine);
                }

                // Disable emission
                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor("_EmissionColor", Color.black);
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    public void OnDetected()
    {
        if (IsDetected) return;
        IsDetected = true;

        Debug.Log($"POI detectado: {name}");

        if (rend != null && revealedMaterial != null)
        {
            Material[] shared = rend.materials;
            uniqueMaterials = new Material[shared.Length];
            emissionTweens = new LTDescr[shared.Length];

            for (int i = 0; i < shared.Length; i++)
            {
                uniqueMaterials[i] = new Material(shared[i]); // Material independiente
            }

            rend.materials = uniqueMaterials;

            for (int i = 0; i < uniqueMaterials.Length; i++)
            {
                Material mat = uniqueMaterials[i];

                // Transición de color
                if (revealedMaterial.HasProperty("_Color") && mat.HasProperty("_Color"))
                {
                    Color startColor = mat.color;
                    Color endColor = revealedMaterial.color;

                    LeanTween.value(gameObject, 0f, 1f, transitionDuration)
                        .setOnUpdate((float val) =>
                        {
                            mat.color = Color.Lerp(startColor, endColor, val);
                        })
                        .setEase(LeanTweenType.easeInOutSine);
                }

                // Emission pulsante
                if (enableEmissionPulse && mat.HasProperty("_EmissionColor"))
                {
                    emissionColor = mat.color;
                    mat.EnableKeyword("_EMISSION");
                    emissionTweens[i] = LeanTween.value(gameObject, 0f, 1f, 1f / emissionPulseSpeed)
                        .setLoopPingPong()
                        .setOnUpdate((float val) =>
                        {
                            mat.SetColor("_EmissionColor", emissionColor * (val * emissionIntensity));
                        })
                        .setEase(LeanTweenType.easeInOutSine);
                }
            }

            // Puedes instanciar partículas si lo deseas aquí
        }
    }

    public bool CanInteract()
    {
        //Check if this poi contains any interactions
        if (interactionsInThisPOI == null || interactionsInThisPOI.Length == 0) return false;

        var priorityBasedInteractions = interactionsInThisPOI.OrderByDescending(x => x.Priority).ToArray();

        foreach (var interaction in priorityBasedInteractions)
        {
            //Check if this interaction is the current objective of the questline
            if (interaction.QuestMeetingConditions)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanInteract(out POI_InteractionHandler result)
    {
        result = null;

        //Check if this poi contains any interactions
        if (interactionsInThisPOI == null || interactionsInThisPOI.Length == 0) return false;

        var priorityBasedInteractions = interactionsInThisPOI.OrderByDescending(x => x.Priority).ToArray();

        foreach (var interaction in priorityBasedInteractions)
        {
            //Check if this interaction is the current objective of the questline
            if (interaction.RequireQuest)
            {
                if (interaction.QuestMeetingConditions)
                {
                    //Check if was activated and if its repetable

                    if (interaction.IsRepeatable)
                    {
                        result = interaction;
                        return true;
                    }
                    else
                    {
                        var gameData = SaveSystem.SaveHandler.GetGameData();

                        if (!gameData.activatedPOIs.Contains(interaction.GetInteractionID))
                        {
                            result = interaction;
                            return true;
                        }
                    }
                }
            }
            else
            {
                //Check if was activated and if its repetable

                if (interaction.IsRepeatable)
                {
                    result = interaction;
                    return true;
                }
                else
                {
                    var gameData = SaveSystem.SaveHandler.GetGameData();

                    if (!gameData.activatedPOIs.Contains(interaction.GetInteractionID))
                    {
                        result = interaction;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        // Detener tweens si el objeto es destruido
        if (emissionTweens != null)
        {
            foreach (var tween in emissionTweens)
            {
                if (tween != null) LeanTween.cancel(tween.id);
            }
        }
    }
}
