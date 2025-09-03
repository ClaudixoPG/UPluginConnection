using UnityEngine;

namespace RythmGame
{
    public class EffectController : MonoBehaviour
    {
        [SerializeField] int timeToDestroy = 1; // Tiempo en segundos para destruir el objeto
                                                // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}