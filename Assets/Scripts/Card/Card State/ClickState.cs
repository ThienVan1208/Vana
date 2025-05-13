using DG.Tweening;
using UnityEngine;

public class ClickState : StateBase
{
    private bool _isUp = false;
    private float _dis2Up = 20f;
    private float _time2Up = 0.2f;
    public ClickState(FSM statemachine, Card card) : base(statemachine, card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (_myCard.cardHolder is HandHolder
        && !(_myCard.cardHolder as HandHolder).CanChooseCard())
        {
            Debug.Log("The chosen card number is out of bound.");
            return;
        }

        isComplete = false;
        _isUp = !_isUp;
        if (_isUp)
        {
            GetUp();
        }
        else
        {
            GetDown();
        }
    }

    private void GetUp()
    {
        (_myCard.cardHolder as HandHolder).ChooseCard(_myCard);

        _myCard.myRect.DOAnchorPosY(_myCard.myRect.localPosition.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(() => _stateMachine.RequestChangeState());
    }
    private void GetDown()
    {
        (_myCard.cardHolder as HandHolder).RejectCard(_myCard);

        _myCard.myRect.DOAnchorPosY(_myCard.myRect.localPosition.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(() => _stateMachine.RequestChangeState());
    }
    public override void OnExit()
    {
        base.OnExit();
        isComplete = true;
    }
}
