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

    void Awake()
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
