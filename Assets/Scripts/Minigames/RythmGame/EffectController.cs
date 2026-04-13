using UnityEngine;

namespace RythmGame
{
    public class EffectController : MonoBehaviour
    {
        [SerializeField] private int timeToDestroy = 1;

        private void Start()
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}