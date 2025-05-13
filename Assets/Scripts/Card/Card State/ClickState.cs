using DG.Tweening;
using UnityEngine;

public class ClickState : StateBase
{
    private bool _isUp = false;
    private float _dis2Up = 15f;
    private float _time2Up = 0.2f;
    public ClickState(FSM statemachine, Card card) : base(statemachine, card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
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
        _myCard.myRect.DOAnchorPosY(_myCard.myRect.position.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);

        if(_myCard.cardHolder is HandHolder){
            (_myCard.cardHolder as HandHolder).ChooseCard(_myCard);
        }
    }
    private void GetDown()
    {
        _myCard.myRect.DOAnchorPosY(_myCard.myRect.position.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);

        if(_myCard.cardHolder is HandHolder){
            (_myCard.cardHolder as HandHolder).RejectCard(_myCard);
        }
    }
}
