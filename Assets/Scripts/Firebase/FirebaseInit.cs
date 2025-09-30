using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using UnityEngine.Events;

public class FirebaseInit : MonoBehaviour
{
    public UnityEvent onFirebaseReady;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase listo!");
                onFirebaseReady.Invoke();
            }
            else
            {
                Debug.LogError($"No se pudo inicializar Firebase: {dependencyStatus}");
            }
        });
    }
}