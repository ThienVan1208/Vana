using Cysharp.Threading.Tasks;
using DG.Tweening;
// using Unity.VisualScripting;
using UnityEngine;

public class ClickState : InteractableState
{
    private bool _isUp = true;

    // Distance to move up when being clicked.
    private float _dis2Up = 20f;

    // Time to move up when being clicked.
    private float _time2Up = 0.2f;

    // If card is chosen to play, this val is true, else is false.
    private bool _chosenFlag = true;
    private bool _scaleLock = false;
    public ClickState(Card card) : base(card)
    {
    }

    public override void OnEnter()
    {
        // Debug.Log("stateclick " + isComplete);
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
    private void ShakeEffect()
    {
        _scaleLock = true;
        Vector3 scaleOff = Vector3.one / 10f;
        CamShakeEvent.RaiseEvent(0.05f, 0.1f);

        myCard.myRect.DOScale(myCard.myRect.localScale + scaleOff, 0.25f).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            myCard.myRect.DOScale(myCard.myRect.localScale - scaleOff, 0.25f).SetEase(Ease.InOutSine);
        });
        myCard.myRect.DOShakeRotation(0.5f, new Vector3(0, 0, 5)).OnComplete(() =>
        {
            _scaleLock = false;
        });

    }
    private void GetUp()
    {
        if (_chosenFlag)
        {
            if (myCard.cardHolder is HandHolder
                && !(myCard.cardHolder as HandHolder).CanChooseCard())
            {
                Debug.Log("Can not choose more card.");
                myCard.stateMachine.RequestChangeState();
                // await UniTask.Yield();
                return;
            }

            // Add card to chosen card list.
            (myCard.cardHolder as HandHolder).ChooseCard(myCard);
            ShakeEffect();
        }

        _isUp = !_isUp;

        

        myCard.backImg.DOAnchorPosY(myCard.backImg.localPosition.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);
        myCard.frontImg.DOAnchorPosY(myCard.frontImg.localPosition.y + _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(async () =>
        {
            await UniTask.WaitUntil(() => !_scaleLock);
            myCard.stateMachine.RequestChangeState();
        });
    }
    private void GetDown()
    {
        _isUp = !_isUp;

        if (_chosenFlag)
        {
            (myCard.cardHolder as HandHolder).RejectCard(myCard);
            ShakeEffect();
        }

        myCard.backImg.DOAnchorPosY(myCard.backImg.localPosition.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad);
        myCard.frontImg.DOAnchorPosY(myCard.frontImg.localPosition.y - _dis2Up, _time2Up)
        .SetEase(Ease.OutQuad).OnComplete(async () =>
        {
            await UniTask.WaitUntil(() => !_scaleLock);

            myCard.stateMachine.RequestChangeState();
        });
    }
    public void SetChosenFlag(bool val)
    {
        _chosenFlag = val;
    }
    public override void OnExit()
    {
        base.OnExit();
        isComplete = true;
        myCard.myRect.localScale = Vector2.one;
    }
    public bool IsClick()
    {
        return _isUp;
    }
}
