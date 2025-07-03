using DG.Tweening;
using UnityEngine;

public class BottomTopPopupEffect : UIEffectBase
{
    [SerializeField] private float _initPosY;
    public override void GetEffect()
    {
        (gameObject.transform as RectTransform).localPosition = new Vector3 (0, _initPosY, 0);
        gameObject.transform.DOLocalMoveY(0, duration).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            gameObject.transform.DOShakePosition(duration, strength: 15, vibrato: 5);
        });
    }

    
}
