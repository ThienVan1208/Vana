
using DG.Tweening;
using UnityEngine;

public abstract class UIEffectBase : MonoBehaviour
{
    [SerializeField] protected float duration;
    public abstract void GetEffect();
    protected virtual void OnDestroy() {
        DOTween.Kill(this);
    }
}
