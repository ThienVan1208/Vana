using UnityEngine;
using DG.Tweening;

public class HoverState : InteractableState
{
    private HandHolder _cardHolder;
    public HoverState(FSM statemachine, Card card) : base(statemachine, card)
    {
    }
    public void SetCardHolder(CardHolder cardHolder){
        if(cardHolder is HandHolder) _cardHolder = (HandHolder)cardHolder;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        isComplete = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(_myCard.myRect.DOShakeRotation(0.2f, new Vector3(0, 0, 5))).OnComplete(() => {
            isComplete = true;
            _stateMachine.RequestChangeState();
        });
        if (_cardHolder == null) Debug.Log("card holder is null");
        if(_cardHolder.IsDrag()) _cardHolder.SetDstCardPointer(_myCard);
        
    }
}
