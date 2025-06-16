using UnityEngine;

public class CenterSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    // Coordenadas de la Escuela de Videojuegos y Realidad Virtual
    private double latitude = -35.40418420558593;
    private double longitude = -71.63177370621082;

    void Start()
    {
        // Obtener el origen del sistema desde el OSMReader
        pLab_OSMReader osmReader = FindObjectOfType<pLab_OSMReader>();
        if (osmReader == null)
        {
            Debug.LogError("No se encontró pLab_OSMReader en la escena.");
            return;
        }

        // Convertir coordenadas geográficas a UTM
        double utmN, utmE;
        pLAB_GeoUtils.LatLongtoUTM(latitude, longitude, out utmN, out utmE);

        // Calcular posición relativa al origen del mapa
        float posX = (float)(utmE - osmReader.UTME_Zero);
        float posZ = (float)(utmN - osmReader.UTMN_Zero);
        Vector3 spawnPosition = new Vector3(posX, 0.5f, posZ);

        // Instanciar el objeto
        GameObject player = playerPrefab != null
            ? Instantiate(playerPrefab, spawnPosition, Quaternion.identity)
            : GameObject.CreatePrimitive(PrimitiveType.Capsule);

        player.name = "Player";
        player.transform.position = spawnPosition;
    }
}
