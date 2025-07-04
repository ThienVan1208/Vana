using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UpDownLoopEffect : UIEffectBase
{
    [SerializeField] private float _distance;
    public async override void GetEffect()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(Random.Range(0, 1.5f)));
        transform.DOLocalMoveY((transform as RectTransform).localPosition.y + _distance / 2, duration)
        .SetEase(Ease.InOutQuad)
        .OnComplete(()=>
        {
            transform.DOLocalMoveY((transform as RectTransform).localPosition.y - _distance / 2, duration)
        .SetEase(Ease.InOutQuad);
        }).SetLoops(-1, LoopType.Yoyo);
    }

    
}
