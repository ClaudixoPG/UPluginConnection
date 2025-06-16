using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float visionAngle = 60f;         // Ángulo del cono
    public float visionDistance = 2f;       // Longitud del cono
    public Color visionGizmoColor = new Color(0f, 0.5f, 1f, 0.2f); // Azul semitransparente

    public List<GameObject> portableObjects;

    public bool IsPOIInVisionCone(PointOfInterest poi)
    {
        Vector3 directionToPOI = poi.transform.position - transform.position;

        // Ignorar altura (trabajar solo en plano XZ)
        Vector3 flatForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 flatDirectionToPOI = new Vector3(directionToPOI.x, 0, directionToPOI.z).normalized;

        float angleToPOI = Vector3.Angle(flatForward, flatDirectionToPOI);

        Debug.Log($"POI: {poi.name}, Angle: {angleToPOI}, Distance: {directionToPOI.magnitude}");

        if (angleToPOI <= visionAngle / 2f && directionToPOI.magnitude <= visionDistance)
        {
            Debug.Log($"POI {poi.name} está dentro del cono de visión del jugador.");
            return true;
        }

        Debug.Log($"POI {poi.name} está fuera del cono de visión del jugador.");
        return false;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Dibuja solo si está seleccionado el jugador en el editor
        if (!Selection.Contains(gameObject)) return;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        Vector3 up = Vector3.up;

        Handles.color = visionGizmoColor;
        Handles.DrawSolidArc(origin, up, Quaternion.Euler(0, -visionAngle / 2f, 0) * forward, visionAngle, visionDistance);

        // Líneas del borde del cono
        Gizmos.color = Color.cyan;
        Vector3 leftLimit = Quaternion.Euler(0, -visionAngle / 2f, 0) * forward * visionDistance;
        Vector3 rightLimit = Quaternion.Euler(0, visionAngle / 2f, 0) * forward * visionDistance;
        Gizmos.DrawLine(origin, origin + leftLimit);
        Gizmos.DrawLine(origin, origin + rightLimit);
    }
    #endif

}
