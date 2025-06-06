using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DragState : MoveableState
{

    private bool _isDrag = false;
    public DragState(Card card) : base(card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

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
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(myCard.myRect
            , Input.mousePosition, Camera.main, out var worldPos))
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
                myCard.stateMachine.RequestChangeState();
                wait4Transit = true;

            }
        }
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
}
