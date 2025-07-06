using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class UpDownLoopEffect : UIEffectBase
{
    [SerializeField] private float _distance;
    public async override void GetEffect(Action callback = null)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0, 1.5f)));
        transform.DOLocalMoveY((transform as RectTransform).localPosition.y + _distance / 2, duration)
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            transform.DOLocalMoveY((transform as RectTransform).localPosition.y - _distance / 2, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                callback?.Invoke();
            });
        }).SetLoops(-1, LoopType.Yoyo);
    }


}
