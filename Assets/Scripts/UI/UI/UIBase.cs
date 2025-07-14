using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBehaviour
{
    public bool playOnAwake = false, playOnStart = true, PlayOnEnable = false, PlayOnDisable = false;
    [SerializeField] protected UnityEvent DisplayEffectOnStart;
    protected virtual void Awake()
    {
        if (playOnAwake) DisplayEffectOnStart?.Invoke();
    }
    protected virtual void OnEnable() {
        if (PlayOnEnable) DisplayEffectOnStart?.Invoke();
    }
    protected virtual void OnDisable()
    {
        if (PlayOnDisable) DisplayEffectOnStart?.Invoke();
    }
    protected virtual void Start()
    {
        if (playOnStart) DisplayEffectOnStart?.Invoke();
    }
}
