using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TileLoader : MonoBehaviour
{
    public int zoom = 16;
    public float xTile = 32701;
    public float yTile = 49925;


    void Start()
    {
        StartCoroutine(LoadTile());
    }

    IEnumerator LoadTile()
    {
        string url = $"https://tile.openstreetmap.org/{zoom}/{xTile}/{yTile}.png";
        Debug.Log("Cargando tile desde: " + url);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tileTexture = DownloadHandlerTexture.GetContent(request);
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = tileTexture;
        }
        else
        {
            Debug.LogError("Error cargando tile: " + request.error);
        }
    }
}
