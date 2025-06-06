using UnityEngine;

public class InteractableState : StateBase
{
    public InteractableState(Card card) : base( card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        if (!myCard.IsInteractable()) return;
    }

    public override void OnExit()
    {
        base.OnExit();
        if (!myCard.IsInteractable()) return;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!myCard.IsInteractable()) return;
    }
}
