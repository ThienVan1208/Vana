using UnityEngine;
using UnityEngine.UI;

public class MoveableState : InteractableState
{
    protected Vector3 _rotateDeg = Vector3.zero;
    protected Vector3 _velocity = Vector3.zero;
    protected Image _cardImg;


    public MoveableState(Card card) : base(card)
    {
        _cardImg = card.GetComponent<Image>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        isComplete = false;

        _cardImg.raycastTarget = false;
        myCard.myRect.SetParent(myCard.cardSlotRect.root);
        myCard.myRect.SetAsLastSibling();
    }
    public override void OnExit()
    {
        base.OnExit();


        wait4Transit = false;
        // myCard.myRect.position = myCard.cardSlotRect.position;

        _cardImg.raycastTarget = true;
        myCard.myRect.SetParent(myCard.cardSlotRect);
        isComplete = true;
    }
    protected void GetMoveEffect(Vector2 target)
    {
        myCard.myRect.position = Vector3.SmoothDamp(myCard.myRect.position, target, ref _velocity, 0.1f);
        //_myRect.position = Vector2.Lerp(_myRect.position, target, Time.deltaTime * 20);
    }
    protected void GetRotateEffect(Vector2 target)
    {
        float dis = Vector2.Distance(myCard.myRect.position, target);
        float deg = myCard.gameConfigSO.cardRotateAngle;

        float lerpRange = dis / myCard.gameConfigSO.cardRotateSpeed;
        if (target.x < myCard.myRect.position.x) _rotateDeg.z = Mathf.Lerp(0, deg, lerpRange);
        else _rotateDeg.z = Mathf.Lerp(0, -deg, lerpRange);
        myCard.myRect.eulerAngles = _rotateDeg;
    }

}
