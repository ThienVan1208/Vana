using DG.Tweening;
using UnityEngine;

public class ShakeClickedButtonEffect : UIEffectBase
{
    public override void GetEffect()
    {
        Vector3 scaleOff = Vector3.one / 2f;
        transform.DOScale(transform.localScale + scaleOff, duration).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            transform.DOScale(transform.localScale - scaleOff, duration).SetEase(Ease.InOutSine);
        });
        
        transform.DOShakeRotation(2 * duration, strength: new Vector3(0, 0, 3), vibrato: 10);
    }

    
}
