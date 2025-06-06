using UnityEngine;

public class MoveToTargetState : MoveableState
{
    public MoveToTargetState(Card card) : base( card)
    {
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        GetMoveEffect(myCard.cardSlotRect.position);
        GetRotateEffect(myCard.cardSlotRect.position);
        if (Vector2.Distance(myCard.cardSlotRect.position, myCard.myRect.position) < 0.001f)
        {
            myCard.stateMachine.RequestChangeState();
            wait4Transit = true;
        }
    }
    
}
