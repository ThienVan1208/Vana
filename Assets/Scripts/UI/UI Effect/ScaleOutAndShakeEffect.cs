using DG.Tweening;
using UnityEngine;

public class ScaleOutAndShakeEffect : UIEffectBase
{
    [SerializeField] private Vector3 _scaleOut = Vector3.one;
    public override void GetEffect()
    {
        transform.DOScale(transform.localScale + _scaleOut, duration).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            transform.DOScale(transform.localScale - _scaleOut, duration).SetEase(Ease.InOutSine);
        });

        transform.DOShakeRotation(2 * duration, strength: new Vector3(0, 0, 3), vibrato: 10);
    }

    
}
