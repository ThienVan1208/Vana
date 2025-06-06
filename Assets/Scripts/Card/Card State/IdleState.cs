using DG.Tweening;
using UnityEngine;

public class IdleState : StateBase
{
    private float _ranDir;
    private Vector2 rotate = Vector2.zero;
    private float _strenght = 10f;
    private float _speed = 1.5f;
    public IdleState(Card card) : base( card)
    {
    }
    public override void OnEnter(){
        base.OnEnter();
        isComplete = true;
        if(myCard.cardSlotRect != null) myCard.myRect.position = myCard.cardSlotRect.position;
        _ranDir = Random.Range(-3f, 4f);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        GetIdlde();
    }
    private void GetIdlde(){

        rotate.x = Mathf.Sin((Time.realtimeSinceStartup + _ranDir) * _speed) * _strenght;
        rotate.y = Mathf.Cos((Time.realtimeSinceStartup + _ranDir) * _speed) * _strenght;
        myCard.myRect.localEulerAngles = rotate;
    }
}
