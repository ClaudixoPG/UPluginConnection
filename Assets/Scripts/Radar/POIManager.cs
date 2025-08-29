using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    private static POIManager instance;

    public static POIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<POIManager>();

            return instance;
        }
    }

    private List<PointOfInterest> allPOIs = new List<PointOfInterest>();

    /// <summary>
    /// Retrieves a Point of Interest (POI) by its unique ID.
    /// Returns null if no matching POI is found.
    /// </summary>
    public PointOfInterest GetPOI(string poi_id)
    {
        return allPOIs.FirstOrDefault(x => x.ID == poi_id);
    }

    //private void Awake()
    //{
    //    if (Instance != null)
    //        Destroy(gameObject);
    //}

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
