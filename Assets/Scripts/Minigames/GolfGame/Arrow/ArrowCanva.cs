using UnityEngine;
using UnityEngine.UI;
public class ArrowCanva : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetFill(float normalizedForce)
    {
        slider.value = normalizedForce; 
    }
}
