using UnityEngine;

public class InteractableState : StateBase
{
    public InteractableState(FSM statemachine, Card card) : base(statemachine, card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (!_myCard.IsInteractable()) return;
    }

    public override void OnExit()
    {
        base.OnExit();
        if (!_myCard.IsInteractable()) return;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_myCard.IsInteractable()) return;
    }
}
