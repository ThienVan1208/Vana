using DG.Tweening;
using UnityEngine;

public class ClickState : InteractableState
{
    private bool _isUp = true;
    private float _dis2Up = 20f;
    private float _time2Up = 0.2f;
    private bool _chosenFlag = true;
    public ClickState(Card card) : base(card)
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
            if (myCard.cardHolder is HandHolder
                && !(myCard.cardHolder as HandHolder).CanChooseCard())
            {
                Debug.Log("Can not choose more card.");
                OnExit();
                return;
            }

            // Add card to chosen card list.
            (myCard.cardHolder as HandHolder).ChooseCard(myCard);
        }

        _isUp = !_isUp;

        // _psEffect = ObjectPoolManager.Instance?.GetPoolingObject<CardPSEffect>()?.GetGlowEffect(myCard.frontImg.transform);

        myCard.backImg.DOAnchorPosY(myCard.backImg.localPosition.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);
        myCard.frontImg.DOAnchorPosY(myCard.frontImg.localPosition.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(() => myCard.stateMachine.RequestChangeState());
    }
    private void GetDown()
    {
        _isUp = !_isUp;

        if (_chosenFlag) (myCard.cardHolder as HandHolder).RejectCard(myCard);
    
        // ObjectPoolManager.Instance?.ReturnToPool<CardPSEffect, ParticleSystem>(_psEffect, isInactive: true);

        myCard.backImg.DOAnchorPosY(myCard.backImg.localPosition.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);
        myCard.frontImg.DOAnchorPosY(myCard.frontImg.localPosition.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(() => myCard.stateMachine.RequestChangeState());
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
