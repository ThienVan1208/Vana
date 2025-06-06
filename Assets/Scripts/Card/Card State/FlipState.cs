using DG.Tweening;
using UnityEngine;

public class FlipState : InteractableState
{
    public FlipState(Card card) : base(card)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        isComplete = false;
        Sequence seq = DOTween.Sequence();
        Vector3 scaleOff = Vector3.one / 2f;
        myCard.myRect.DOScale(myCard.myRect.localScale + scaleOff, 0.25f).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            myCard.myRect.DOScale(myCard.myRect.localScale - scaleOff, 0.25f).SetEase(Ease.InOutSine);
        });
        seq.Append(myCard.myRect.DOShakeRotation(0.5f, new Vector3(0, 0, 10))).OnComplete(() =>
        {
            isComplete = true;
            myCard.stateMachine.RequestChangeState();
        });
    }
    
}
