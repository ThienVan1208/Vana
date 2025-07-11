using System;
using DG.Tweening;
using UnityEngine;

public class BottomTopPopupEffect : UIEffectBase
{
    public float startYPos;
    public float endYPos = 0;
    public bool shakeAtEnd = true;
    public override void GetEffect(Action callback = null)
    {
        (gameObject.transform as RectTransform).localPosition = new Vector3(0, startYPos, 0);
        gameObject.transform.DOLocalMoveY(endYPos, duration).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            if (shakeAtEnd)
            {
                gameObject.transform.DOShakePosition(duration, strength: 15, vibrato: 5).OnComplete(() =>
                {
                    callback?.Invoke();
                });
            }
            else
            {
                callback?.Invoke();
            }

        });
    }


}
