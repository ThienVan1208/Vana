using DG.Tweening;
using UnityEngine;

public class ClickState : InteractableState
{
    private bool _isUp = true;
    private float _dis2Up = 20f;
    private float _time2Up = 0.2f;
    private bool _chosenFlag = true;
    public ClickState(FSM statemachine, Card card) : base(statemachine, card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        isComplete = false;

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
        if (_chosenFlag)
        {
            if (_myCard.cardHolder is HandHolder
                && !(_myCard.cardHolder as HandHolder).CanChooseCard())
            {
                Debug.Log("Can not choose more card.");
                OnExit();
                return;
            }

            // Add card to chosen card list.
            (_myCard.cardHolder as HandHolder).ChooseCard(_myCard);
        }

        _isUp = !_isUp;
        
        _myCard.backImg.DOAnchorPosY(_myCard.backImg.localPosition.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);
        _myCard.frontImg.DOAnchorPosY(_myCard.frontImg.localPosition.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(() => _stateMachine.RequestChangeState());
    }
    private void GetDown()
    {
        _isUp = !_isUp;

        if (_chosenFlag) (_myCard.cardHolder as HandHolder).RejectCard(_myCard);

        _myCard.backImg.DOAnchorPosY(_myCard.backImg.localPosition.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);
        _myCard.frontImg.DOAnchorPosY(_myCard.frontImg.localPosition.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(() => _stateMachine.RequestChangeState());
    }
    public void SetChosenFlag(bool val)
    {
        _chosenFlag = val;
    }
    public override void OnExit()
    {
        base.OnExit();
        isComplete = true;

    }
    public bool IsClick()
    {
        return _isUp;
    }
}
