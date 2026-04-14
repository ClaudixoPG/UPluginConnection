using UnityEngine;

public class AutoDestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.7f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}