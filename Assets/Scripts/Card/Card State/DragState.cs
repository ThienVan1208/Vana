using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DragState : MoveableState
{

    private bool _isDrag = false;
    private HandHolder _cardHolder;
    public DragState(FSM fsm, Card card) : base(fsm, card)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        _isDrag = true;
        _cardHolder.SetDrag(_isDrag);
        _cardHolder.SetSrcCardPointer(_myCard);
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
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_myCard.myRect
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
    
            GetMoveEffect(_myCard.cardSlotRect.position);
            GetRotateEffect(_myCard.cardSlotRect.position);
            if (Vector2.Distance(_myCard.cardSlotRect.position, _myCard.myRect.position) < 0.001f)
            {
                _stateMachine.RequestChangeState();
                //isComplete = true;
                _wait4Transit = true;
            
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
        if (!_myCard.IsInteractable()) return;
        _cardHolder.SetDrag(false);
        
    }
    public void SetCardHolder(CardHolder cardHolder)
    {
        if(cardHolder is HandHolder) _cardHolder = (HandHolder)cardHolder;
    }
    public override void OnExit()
    {
        base.OnExit();
        //_cardHolder.UpdateCardVsSlot();
    }
}
