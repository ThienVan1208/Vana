using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBehaviour
{
    [SerializeField] protected UnityEvent DisplayEffectOnStart;
    protected virtual void Start()
    {
        DisplayEffectOnStart?.Invoke();
    }
}
