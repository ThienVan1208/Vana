using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine.UI;
public class HoverState : StateBase
{
    private HandHolder _cardHolder;
    public HoverState(FSM statemachine, Card card) : base(statemachine, card)
    {
    }
    public void SetCardHolder(CardHolder cardHolder){
        _cardHolder = (HandHolder)cardHolder;
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

        if(_cardHolder.IsDrag()) _cardHolder.SetDstCardPointer(_myCard);
        
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        
    }
}
