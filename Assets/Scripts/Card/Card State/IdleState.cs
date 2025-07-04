
using UnityEngine;


public class IdleState : StateBase
{
    private float _ranDir;
    private Vector2 rotate = Vector2.zero;
    private float _strenght = 15f;
    private float _speed = 1.5f;
    private Vector3 _moveDir;
    public bool didIdle = true;
    public IdleState(Card card) : base(card)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        if (!didIdle)
        {
            myCard.myRect.localEulerAngles = Vector3.zero;
        }
        myCard.myRect.localScale = Vector2.one;
        isComplete = true;
        if (myCard.cardSlotRect != null) myCard.myRect.position = myCard.cardSlotRect.position;
        _ranDir = Random.Range(-4f, 5f);
    }

    public override void OnExit()
    {
        base.OnExit();
        // if (myCard.cardSlotRect != null) myCard.myRect.position = myCard.cardSlotRect.position;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!didIdle) return;
        GetRotateEffect();
        GetMoveEffect();
    }
    private void GetRotateEffect()
    {
        rotate.x = Mathf.Sin((Time.realtimeSinceStartup + _ranDir) * _speed) * _strenght;
        rotate.y = Mathf.Cos((Time.realtimeSinceStartup + _ranDir) * _speed) * _strenght;
        myCard.myRect.localEulerAngles = rotate;
    }
    private void GetMoveEffect()
    {
        _moveDir.y = Mathf.Sin(Time.realtimeSinceStartup + _ranDir) / 350f;
        // Debug.Log(_moveDir.y);
        myCard.myRect.localPosition += _moveDir;
    }
}
