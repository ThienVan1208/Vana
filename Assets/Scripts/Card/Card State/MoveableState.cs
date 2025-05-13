using UnityEngine;
using UnityEngine.UI;

public class MoveableState : StateBase
{
    protected Vector3 _rotateDeg = Vector3.zero;
    protected Vector3 _velocity = Vector3.zero;

    protected Image _cardImg;
    public MoveableState(FSM statemachine, Card card) : base(statemachine, card)
    {
        _cardImg = card.GetComponent<Image>();
    }
    
    public override void OnEnter(){
        base.OnEnter();
        isComplete = false;

        _cardImg.raycastTarget = false;
        _myCard.myRect.SetParent(_myCard.cardSlotRect.root);
        _myCard.myRect.SetAsLastSibling();
    }
    public override void OnExit()
    {
        base.OnExit();
        isComplete = true;

        _wait4Transit = false;
        _myCard.myRect.position = _myCard.cardSlotRect.position;
        
        _cardImg.raycastTarget = true;
        _myCard.myRect.SetParent(_myCard.cardSlotRect);
    }
    protected void GetMoveEffect(Vector2 target)
    {
        _myCard.myRect.position = Vector3.SmoothDamp(_myCard.myRect.position, target, ref _velocity, 0.05f);
        //_myRect.position = Vector2.Lerp(_myRect.position, target, Time.deltaTime * 20);
    }
    protected void GetRotateEffect(Vector2 target)
    {
        float dis = Vector2.Distance(_myCard.myRect.position, target);
        float deg = 60f;
        float lerpRange = dis / 100f;
        if (target.x < _myCard.myRect.position.x) _rotateDeg.z = Mathf.Lerp(0, deg, lerpRange);
        else _rotateDeg.z = Mathf.Lerp(0, -deg, lerpRange);
        _myCard.myRect.eulerAngles = _rotateDeg;
    }
    
}
