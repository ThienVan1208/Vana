using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class JumpOffTextEffect : TextObjectEffect
{
    [SerializeField] private UIPositionType _typePos;
    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
    public override void GetEffect(TextMeshProUGUI displayText, Vector3 startPos, Vector3 endPos, float duration, Action callback = null)
    {
        if (_typePos == UIPositionType.World)
        {
            displayText.rectTransform.position = startPos;

            displayText.rectTransform.DOScale(displayText.rectTransform.localScale.x + 1, duration / 2)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() =>
            {
                displayText.rectTransform.DOShakePosition(duration / 2, strength: 5f, vibrato: 1);
                displayText.rectTransform.DOScale(displayText.rectTransform.localScale.x - 1, duration / 2)
                .SetEase(Ease.InOutQuart);
            });

            displayText.rectTransform.DOMove(startPos + endPos, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                displayText.rectTransform.DOMoveY(displayText.rectTransform.position.y + endPos.y, duration)
                .SetEase(Ease.InOutQuad);
                displayText.DOFade(0, duration).OnComplete(() => callback?.Invoke());

            });
        }
        else
        {
            displayText.rectTransform.localPosition = startPos;

            displayText.rectTransform.DOScale(displayText.rectTransform.localScale.x + 1, duration / 2)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() =>
            {
                displayText.rectTransform.DOShakePosition(duration / 2, strength: 5f, vibrato: 1);
                displayText.rectTransform.DOScale(displayText.rectTransform.localScale.x - 1, duration / 2)
                .SetEase(Ease.InOutQuart);
            });

            displayText.rectTransform.DOLocalMove(startPos + endPos, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                displayText.rectTransform.DOLocalMoveY(displayText.rectTransform.localPosition.y + endPos.y, duration)
                .SetEase(Ease.InOutQuad);
                displayText.DOFade(0, duration).OnComplete(() => callback?.Invoke());

            });
        }

    }
}
