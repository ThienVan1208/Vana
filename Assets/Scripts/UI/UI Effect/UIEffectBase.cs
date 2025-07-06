using System;
using DG.Tweening;
using UnityEngine;


public abstract class UIEffectBase : MonoBehaviour
{
    public float duration;
    public abstract void GetEffect(Action calback = null);
    public virtual void GetEffect()
    {
        GetEffect(calback: null);
    }
    protected virtual void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
