using System.Collections;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RadarScanner : MonoBehaviour
{
    public enum RadarMode
    {
        Sphere,
        Flat
    }

    [Header("Radar Mode")]
    public bool enableZonesView = true;
    public RadarMode radarMode = RadarMode.Sphere;
    public float maxDistance = 6f;
    public float scanInterval = 0.5f;
    public Color farColor, midColor, nearColor;

    private float scanTimer;
    private float farDistance, midDistance, nearDistance;

    [Header("Radar Wave Settings")]
    public bool enableRadarWave = true;
    public float minSpeed = 2f;
    public float maxSpeed = 6f;

    [Header("Radar Wave Colors")]
    public Color noDetectionColor = Color.white;
    public Color farZoneColor = Color.red;
    public Color midZoneColor = Color.yellow;
    public Color nearZoneColor = Color.green;

    // Variable interna para almacenar color actual del pulso
    public Color currentWaveColor = Color.white;
    private LTDescr colorTween;
    public float waveSpeed { get; private set; }
    public float waveWidth = 0.2f;

    private float currentWaveRadius;

    //MESH
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh radarMesh;

    /// <summary>
    /// respaldo
    /// </summary>

    private bool colorNeedsUpdate = false;

    // Parámetros para animación
    float minAmplitude = 0.05f;
    float maxAmplitude = 1f;
    float minFrequency = 1f;   // Lento cuando está lejos
    float maxFrequency = 20f;   // Rápido cuando está cerca

    private void Start()
    {
        UpdateDistances();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        radarMesh = new Mesh();
        meshFilter.mesh = radarMesh;

        Material dynamicMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        dynamicMat.color = currentWaveColor; // Inicializar con el color actual
        meshRenderer.material = dynamicMat;

    }

    /*private void UpdateRadarWaveMesh()
    {
        if (!enableRadarWave || radarMode != RadarMode.Flat) return;

        int segments = 128;
        float time = Time.time;
        float radius = currentWaveRadius;

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        // Cambiar a blanco temporalmente, luego ajusta según la detección
        meshRenderer.material.color = currentWaveColor; // Actualiza el color del material

        vertices[0] = Vector3.zero; // El centro de la onda
        float proximity = 1f - Mathf.Clamp01(GetClosestDistance() / maxDistance);
        proximity = Mathf.Pow(proximity, 1.5f); // más suave y progresiva

        // Cambiar la forma en que se calculan la amplitud y la frecuencia
        float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, proximity);  // Amplitud más suave basada en la proximidad
        float frequency = Mathf.Lerp(minFrequency, maxFrequency, proximity);      // Frecuencia más rápida cuando el objetivo está cerca

        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // El valor Y será una onda sinusoidal con frecuencia dinámica
            float y = Mathf.Sin(angle * frequency + time * frequency) * amplitude;

            vertices[i + 1] = new Vector3(x, y, z);

            // Triangulación de los vértices
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2 > segments) ? 1 : i + 2;
        }

        radarMesh.Clear();
        radarMesh.vertices = vertices;
        radarMesh.triangles = triangles;
        radarMesh.RecalculateNormals();
    }*/
    /*private void UpdateRadarWaveMesh()
    {
        if (!enableRadarWave || radarMode != RadarMode.Flat) return;

        int segments = 128;
        float time = Time.time;
        float radius = currentWaveRadius;

        // Parámetro de grosor
        float thickness = 0.1f;  // Puedes ajustar este valor para cambiar el grosor del borde
        float innerRadius = radius - thickness;  // Radio interior para el borde

        // Vértices para el borde (dos círculos concéntricos)
        Vector3[] vertices = new Vector3[segments * 2]; // Dos círculos
        int[] triangles = new int[segments * 6]; // Se necesitan 2 triángulos por segmento

        // Calculamos las posiciones de los vértices para el borde (con 2 radios)
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;

            // Radio exterior
            float xOuter = Mathf.Cos(angle) * radius;
            float zOuter = Mathf.Sin(angle) * radius;
            vertices[i] = new Vector3(xOuter, 0, zOuter);

            // Radio interior
            float xInner = Mathf.Cos(angle) * innerRadius;
            float zInner = Mathf.Sin(angle) * innerRadius;
            vertices[i + segments] = new Vector3(xInner, 0, zInner);
        }

        // Creamos los triángulos conectando los vértices de los dos círculos
        for (int i = 0; i < segments - 1; i++)
        {
            // Triángulos entre el vértice del círculo exterior y el interior
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + segments;
            triangles[i * 6 + 2] = i + 1;

            triangles[i * 6 + 3] = i + 1;
            triangles[i * 6 + 4] = i + segments;
            triangles[i * 6 + 5] = i + segments + 1;
        }

        // Cerramos el círculo conectando el último vértice con el primero
        triangles[segments * 6 - 6] = segments - 1;
        triangles[segments * 6 - 5] = segments * 2 - 1;
        triangles[segments * 6 - 4] = 0;

        triangles[segments * 6 - 3] = 0;
        triangles[segments * 6 - 2] = segments * 2 - 1;
        triangles[segments * 6 - 1] = segments;

        radarMesh.Clear();
        radarMesh.vertices = vertices;
        radarMesh.triangles = triangles;
        radarMesh.RecalculateNormals();  // Recalculamos las normales para una correcta visualización

        // Aplicamos el color dinámico al material
        meshRenderer.material.color = currentWaveColor;
    }*/


    private void UpdateRadarWaveMesh()
    {
        if (!enableRadarWave || radarMode != RadarMode.Flat) return;

        int segments = 128;
        float time = Time.time;
        float radius = currentWaveRadius;

        // Parámetro de grosor
        float thickness = 0.1f;  // Puedes ajustar este valor para cambiar el grosor del borde
        float innerRadius = radius - thickness;  // Radio interior para el borde

        // Vértices para el borde (dos círculos concéntricos)
        Vector3[] vertices = new Vector3[segments * 2]; // Dos círculos
        int[] triangles = new int[segments * 6]; // Se necesitan 2 triángulos por segmento

        // Cambiar a blanco temporalmente, luego ajusta según la detección
        meshRenderer.material.color = currentWaveColor; // Actualiza el color del material

        float proximity = 1f - Mathf.Clamp01(GetClosestDistance() / maxDistance);
        proximity = Mathf.Pow(proximity, 1.5f); // Más suave y progresiva

        // Cambiar la forma en que se calculan la amplitud y la frecuencia
        float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, proximity);  // Amplitud más suave basada en la proximidad
        float frequency = Mathf.Lerp(minFrequency, maxFrequency, proximity);      // Frecuencia más rápida cuando el objetivo está cerca

        // Calculamos las posiciones de los vértices para el borde (con 2 radios)
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;

            // Radio exterior
            float xOuter = Mathf.Cos(angle) * radius;
            float zOuter = Mathf.Sin(angle) * radius;
            float yOuter = Mathf.Sin(angle * frequency + time * frequency) * amplitude;
            vertices[i] = new Vector3(xOuter, yOuter, zOuter);

            // Radio interior
            float xInner = Mathf.Cos(angle) * innerRadius;
            float zInner = Mathf.Sin(angle) * innerRadius;
            float yInner = Mathf.Sin(angle * frequency + time * frequency) * amplitude;
            vertices[i + segments] = new Vector3(xInner, yInner, zInner);
        }

        // Creamos los triángulos conectando los vértices de los dos círculos
        for (int i = 0; i < segments - 1; i++)
        {
            // Triángulos entre el vértice del círculo exterior y el interior
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + segments;
            triangles[i * 6 + 2] = i + 1;

            triangles[i * 6 + 3] = i + 1;
            triangles[i * 6 + 4] = i + segments;
            triangles[i * 6 + 5] = i + segments + 1;
        }

        // Cerramos el círculo conectando el último vértice con el primero
        triangles[segments * 6 - 6] = segments - 1;
        triangles[segments * 6 - 5] = segments * 2 - 1;
        triangles[segments * 6 - 4] = 0;

        triangles[segments * 6 - 3] = 0;
        triangles[segments * 6 - 2] = segments * 2 - 1;
        triangles[segments * 6 - 1] = segments;

        radarMesh.Clear();
        radarMesh.vertices = vertices;
        radarMesh.triangles = triangles;
        radarMesh.RecalculateNormals();  // Recalculamos las normales para una correcta visualización

        // Aplicamos el color dinámico al material
        meshRenderer.material.color = currentWaveColor;
    }



    private void Update()
    {
        scanTimer += Time.deltaTime;

        if (enableRadarWave)
        {
            currentWaveRadius += waveSpeed * Time.deltaTime;
            if (currentWaveRadius > farDistance)
            {
                currentWaveRadius = 0f; // reiniciar la onda
            }
        }

        if (scanTimer >= scanInterval)
        {
            scanTimer = 0f;
            ScanForPOIs();
            colorNeedsUpdate = true; // fuerza actualización tras escaneo
        }
        UpdateRadarWaveMesh();
    }

    private float GetClosestDistance()
    {
        float closest = maxDistance;
        if (POIManager.Instance != null)
        {
            foreach (var poi in POIManager.Instance.GetPOIsInRange(transform.position, maxDistance))
            {
                float d = Vector3.Distance(transform.position, poi.transform.position);
                if (d < closest) closest = d;
            }
        }
        return closest;
    }
    private void UpdateDistances()
    {
        float unit = maxDistance / 6f;
        farDistance = unit * 6f;
        midDistance = unit * 4f;
        nearDistance = unit * 2f;
    }
    private void ScanForPOIs()
    {
        if (POIManager.Instance == null)
        {
            Debug.LogWarning("No hay POIManager en escena.");
            return;
        }

        var pois = POIManager.Instance.GetPOIsInRange(transform.position, farDistance);

        if (pois.Count() == 0)
        {
            // Nada detectado → color blanco, velocidad mínima
            currentWaveColor = noDetectionColor;
            waveSpeed = minSpeed;
            return;
        }

        float closestDistance = float.MaxValue;
        foreach (var poi in pois)
        {
            float dist = Vector3.Distance(transform.position, poi.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
            }

            //get parent
            var player = GameObject.FindGameObjectWithTag("Player");
            bool isInPlayerVision = player.GetComponent<PlayerController>().IsPOIInVisionCone(poi);

            if(dist<=nearDistance && !isInPlayerVision)
            {
                //Debug.Log($"POI en zona cercana pero fuera de la visión del jugador: {poi.name} a {dist:F2} unidades");
            }
            if (dist <= nearDistance && isInPlayerVision)
            {
                Debug.Log($"Detectado en zona cercana: {poi.name}");
                POIManager.Instance.MarkAsDetected(poi);
            }
            else
            {
                //Debug.Log($"POI dentro de radar pero fuera de zona cercana: {poi.name} a {dist:F2} unidades");
            }
        }

        UpdateWaveSpeed(closestDistance);
        UpdateColorZone(closestDistance);
    }
    private void UpdateWaveSpeed(float distancia)
    {
        float t = 1f - Mathf.Clamp01(distancia / farDistance); // Cerca → 1, Lejos → 0
        waveSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);
    }
    private void UpdateColorZone(float distance)
    {
        Color targetColor;

        if (distance > farDistance)
        {
            targetColor = noDetectionColor;
        }
        else if (distance > midDistance)
        {
            targetColor = farZoneColor;
        }
        else if (distance > nearDistance)
        {
            targetColor = midZoneColor;
        }
        else
        {
            targetColor = nearZoneColor;
        }

        // Evita crear múltiples tweens simultáneamente
        if (colorTween != null && LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject, colorTween.uniqueId);

        // Inicia tween hacia el nuevo color
        /*colorTween = LeanTween.value(gameObject, currentWaveColor, targetColor, 1f) // duración en segundos
            .setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate((Color val) =>
            {
                currentWaveColor = val;
            });*/

        colorTween = LeanTween.value(gameObject, currentWaveColor, targetColor, 1f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate((Color val) =>
            {
                currentWaveColor = val;
                colorNeedsUpdate = true;
            });
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UpdateDistances();
        Vector3 position = transform.position;

        if(enableZonesView)
        {
            if (radarMode == RadarMode.Sphere)
            {
                Gizmos.color = farColor;
                Gizmos.DrawSphere(position, farDistance);

                Gizmos.color = midColor;
                Gizmos.DrawSphere(position, midDistance);

                Gizmos.color = nearColor;
                Gizmos.DrawSphere(position, nearDistance);
            }
            else if (radarMode == RadarMode.Flat)
            {
                Vector3 up = Vector3.up;
                Handles.color = farColor;
                Handles.DrawSolidArc(position, up, Vector3.forward, 360f, farDistance);

                Handles.color = midColor;
                Handles.DrawSolidArc(position, up, Vector3.forward, 360f, midDistance);

                Handles.color = nearColor;
                Handles.DrawSolidArc(position, up, Vector3.forward, 360f, nearDistance);
            }
        }

        if (enableRadarWave)
        {

            /*Handles.color = currentWaveColor;
            Handles.DrawWireDisc(position, Vector3.up, currentWaveRadius);
            Handles.DrawWireDisc(position, Vector3.up, currentWaveRadius + waveWidth);*/

            int segments = 128;
            float time = Time.realtimeSinceStartup;
            float baseRadius = currentWaveRadius;

            // Normalizar la proximidad
            float proximity = 1f - Mathf.Clamp01(GetClosestDistance() / maxDistance);
            proximity = Mathf.Pow(proximity, 1.5f); // más suave y progresiva

            float dynamicAmplitude = Mathf.Lerp(minAmplitude, maxAmplitude, proximity);


            float dynamicFrequency = Mathf.Lerp(minFrequency, maxFrequency, proximity);

            // Dibujar círculo con movimiento vertical sinusoidal
            Vector3[] points = new Vector3[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float angle = (2 * Mathf.PI / segments) * i;

                float x = Mathf.Cos(angle) * baseRadius;
                float z = Mathf.Sin(angle) * baseRadius;

                // Movimiento en Y: A * sin(ω * t + φ)
                float y = Mathf.Sin(angle * dynamicFrequency + time * dynamicFrequency * 2f) * dynamicAmplitude;

                points[i] = transform.position + new Vector3(x, y, z);
            }

            Handles.color = currentWaveColor;
            Handles.DrawAAPolyLine(2f, points);

        }


    }
#endif
}
