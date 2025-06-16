using System.Collections;
using UnityEngine;
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

    public RadarMode radarMode = RadarMode.Sphere;
    public float maxDistance = 6f;
    public float scanInterval = 0.5f;
    public Color farColor, midColor, nearColor;

    private float scanTimer;
    private float farDistance, midDistance, nearDistance;

    private void Start()
    {
        UpdateDistances();
    }

    private void Update()
    {
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanInterval)
        {
            scanTimer = 0f;
            ScanForPOIs();
        }
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

        foreach (var poi in pois)
        {
            float dist = Vector3.Distance(transform.position, poi.transform.position);

            //get parent
            var player = GameObject.FindGameObjectWithTag("Player");
            bool isInPlayerVision = player.GetComponent<PlayerController>().IsPOIInVisionCone(poi);

            /*if(dist<=nearDistance && !isInPlayerVision)
            {
                Debug.Log($"POI en zona cercana pero fuera de la visión del jugador: {poi.name} a {dist:F2} unidades");
            }*/
            if (dist <= nearDistance && isInPlayerVision)
            {
                Debug.Log($"Detectado en zona cercana: {poi.name}");
                POIManager.Instance.MarkAsDetected(poi);
            }
            else
            {
                Debug.Log($"POI dentro de radar pero fuera de zona cercana: {poi.name} a {dist:F2} unidades");
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UpdateDistances();
        Vector3 position = transform.position;

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
#endif
}
