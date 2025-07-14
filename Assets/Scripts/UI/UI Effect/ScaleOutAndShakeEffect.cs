using System;
using DG.Tweening;
using UnityEngine;


public class ScaleOutAndShakeEffect : UIEffectBase
{
    [SerializeField] private Vector3 _extraScaleOut = Vector3.one;
    private bool _lockEffect = false;
    public override void GetEffect(Action callback = null, bool reverse = false)
    {
        if (_lockEffect) return;

        _lockEffect = true;
        transform.DOScale(transform.localScale + _extraScaleOut, duration).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            transform.DOScale(transform.localScale - _extraScaleOut, duration).SetEase(Ease.InOutSine);
        });

        transform.DOShakeRotation(2 * duration, strength: new Vector3(0, 0, 3), vibrato: 10)
        .OnComplete(() =>
        {
            callback?.Invoke();
            _lockEffect = false; 
        });
    }


}
