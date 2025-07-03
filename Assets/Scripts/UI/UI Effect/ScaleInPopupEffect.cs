using DG.Tweening;
using UnityEngine;

public class ScaleInPopupEffect : UIEffectBase
{
    [SerializeField] private float _scale = 1f;
    
    public override void GetEffect()
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(_scale, duration).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            gameObject.transform.DOShakePosition(duration, strength: 15, vibrato: 5);
        });
    }

    
}
