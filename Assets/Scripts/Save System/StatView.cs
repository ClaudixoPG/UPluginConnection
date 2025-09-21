using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace SaveSystem
{
    public abstract class StatView : MonoBehaviour
    {
        public abstract void Display(StadisticsLog stadistic, UnityAction onCompleteDisplay);
    }
}