using UnityEngine;
using UnityEngine.InputSystem;

public class DragState : MoveableState
{

    private bool _isDrag = false;
    private Vector2 _targetPos;
    public DragState(Card card) : base(card)
    {
        InputSystem.Update();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        myCard.interactInputReaderSO.MousePosAction += SetTarget;

        _isDrag = true;
        if (myCard.cardHolder == null || !(myCard.cardHolder is HandHolder)) return;

        (myCard.cardHolder as HandHolder).SetDrag(_isDrag);
        (myCard.cardHolder as HandHolder).SetSrcCardPointer(myCard);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        GetDragEffect();

    }
    private void GetDragEffect()
    {
        // Follow mouse position when start dragging.
        if (_isDrag)
        {
            // Canvas is in overlay mode -> camera = null.

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(myCard.myRect, _targetPos, Camera.main, out var worldPos))
            {
                // Move.
                GetMoveEffect(worldPos);

                // Rotate.
                GetRotateEffect(worldPos);
            }
        }

        // End dragging -> move back to @_cardSlot position.
        else
        {

            GetMoveEffect(myCard.cardSlotRect.position);
            GetRotateEffect(myCard.cardSlotRect.position);
            if (Vector2.Distance(myCard.cardSlotRect.position, myCard.myRect.position) < 0.001f)
            {
                isComplete = true;
                myCard.stateMachine.RequestChangeState();
                wait4Transit = true;

            }
        }
    }
    public override void OnExit()
    {
        base.OnExit();
        myCard.interactInputReaderSO.MousePosAction -= SetTarget;
    }
    public void StartDrag()
    {
        _isDrag = true;
    }
    public void EndDrag()
    {
        _isDrag = false;
        if (!myCard.IsInteractable()) return;
        (myCard.cardHolder as HandHolder).SetDrag(false);

    }
    public void SetTarget(Vector2 target)
    {
        _targetPos = target;
    }
}
