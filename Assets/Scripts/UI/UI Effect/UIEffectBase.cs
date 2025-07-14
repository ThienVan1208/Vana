using System;
using DG.Tweening;
using UnityEngine;


public abstract class UIEffectBase : MonoBehaviour
{
    public float duration;
    public abstract void GetEffect(Action callback = null, bool reverse = false);
    public virtual void GetEffect(bool reverse = false)
    {
        GetEffect(callback: null, reverse: reverse);
    }
    protected virtual void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
