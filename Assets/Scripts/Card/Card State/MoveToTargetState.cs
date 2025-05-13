using UnityEngine;

public class MoveToTargetState : MoveableState
{
    public MoveToTargetState(FSM statemachine, Card card) : base(statemachine, card)
    {
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        GetMoveEffect(_myCard.cardSlotRect.position);
        GetRotateEffect(_myCard.cardSlotRect.position);
        if (Vector2.Distance(_myCard.cardSlotRect.position, _myCard.myRect.position) < 0.01f)
        {
            _stateMachine.RequestChangeState();
            //isComplete = true;
            _wait4Transit = true;
        }
    }
    
}
