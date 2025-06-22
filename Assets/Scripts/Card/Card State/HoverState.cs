using UnityEngine;
using DG.Tweening;

public class HoverState : InteractableState
{

    public HoverState(Card card) : base(card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        isComplete = true;
        // isComplete = false;
        // Sequence seq = DOTween.Sequence();
        // seq.Append(myCard.myRect.DOShakeRotation(0.2f, new Vector3(0, 0, 5))).OnComplete(() =>
        // {
        //     isComplete = true;
        //     myCard.stateMachine.RequestChangeState();
        // });

        if (myCard.cardHolder == null || !(myCard.cardHolder is HandHolder))
        {
            return;
        }

        if ((myCard.cardHolder as HandHolder).IsDrag()) (myCard.cardHolder as HandHolder).SetDstCardPointer(myCard);

    }
}
