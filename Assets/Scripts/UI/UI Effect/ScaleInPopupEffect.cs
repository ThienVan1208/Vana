using System;
using DG.Tweening;
using UnityEngine;


public class ScaleInPopupEffect : UIEffectBase
{
    [SerializeField] private float _scale = 1f;

    public override void GetEffect(Action callback = null, bool reverse = false)
    {
        if (reverse)
        {
            gameObject.transform.DOScale(0.001f, duration).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
        else
        {
            gameObject.transform.localScale = Vector3.zero;
            gameObject.transform.DOScale(_scale, duration).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                gameObject.transform.DOShakePosition(duration, strength: 15, vibrato: 5).OnComplete(() =>
                {
                    callback?.Invoke();
                });
            });
        }

    }


}
