using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public static POIManager Instance;

    private List<PointOfInterest> allPOIs = new List<PointOfInterest>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterPOI(PointOfInterest poi)
    {
        if (!allPOIs.Contains(poi))
        {
            allPOIs.Add(poi);
            //Debug.Log($"POI registrado: {poi.name}");
        }
    }

    public IEnumerable<PointOfInterest> GetPOIsInRange(Vector3 origin, float maxDistance)
    {
        foreach (var poi in allPOIs)
        {
            if (!poi.IsDetected)
            {
                float dist = Vector3.Distance(origin, poi.transform.position);
                if (dist <= maxDistance)
                {
                    //Debug.Log($"POI en rango: {poi.name} a {dist} unidades");
                    yield return poi;
                }
            }
        }
    }

    public void MarkAsDetected(PointOfInterest poi)
    {
        if (!poi.IsDetected)
        {
            Debug.Log($"Marcando como detectado: {poi.name}");
            poi.OnDetected();
        }
    }
}
