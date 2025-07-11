using System;
using DG.Tweening;
using UnityEngine;


public abstract class UIEffectBase : MonoBehaviour
{
    public float duration;
    public abstract void GetEffect(Action callback = null);
    public virtual void GetEffect()
    {
        GetEffect(callback: null);
    }
    protected virtual void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
