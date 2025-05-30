using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandHolder : PlayableCardHolder
{
    [SerializeField] private List<Card> _chosenCards = new List<Card>();
    private Card _srcCardPointer;
    private Card _dstCardPointer;
    private bool _isDrag = false;
    private bool _isSwap = false;
    
    
    // Used thru button.
    public override bool HelpPlayingCard()
    {
        if (_chosenCards.Count < gameConfigSO.minCard2Play)
        {
            Debug.Log("Must choose at least" + gameConfigSO.minCard2Play + "cards");
            return false;
        }

        chosenCardEventSO.RaiseEvent(_chosenCards);

        foreach (var card in _chosenCards)
        {
            // Disconnect from current cardHolder.
            _cardsDic[card.cardSlotRect] = null;
            curCardNum--;

            /* 
            - The distance between 2 cards is the distance that 
                the UI panel contains gameConfigSO.initCardNum elements.
            - So if the current card number is over gameConfigSO.initCardNum -> active it false.
            */
            if (curCardNum > gameConfigSO.initCardNum)
            {
                GetCardSlot(card)?.gameObject.SetActive(false);
            }
        }

        // Clear chosen card list for the next choosing turn.
        _chosenCards.Clear();
        return true;
    }
    public void SetSrcCardPointer(Card card)
    {
        if (!_isDrag) return;
        _srcCardPointer = card;
    }
    public void SetDstCardPointer(Card card)
    {
        if (!_isDrag) return;

        _dstCardPointer = card;
        SwapCard();
    }
    public bool IsDrag()
    {
        return _isDrag;
    }
    public void SetDrag(bool isDrag)
    {
        _isDrag = isDrag;
    }

    public void UpdateCardVsSlot()
    {
        foreach (var cardSlotPair in _cardsDic)
        {
            RectTransform slot = cardSlotPair.Key;
            Card card = cardSlotPair.Value;
            (card.transform as RectTransform).position = card.cardSlotRect.position;
        }
    }
    public void SwapCard()
    {
        if (_isSwap) return;

        _isSwap = true;
        int srcIndex = GetIndexOfCardSlot(_srcCardPointer.cardSlotRect);
        int dstIndex = GetIndexOfCardSlot(_dstCardPointer.cardSlotRect);
        //GetCard(_srcCardPointer.cardSlotRect).SetCardSlot(_dstCardPointer.cardSlotRect);
        _srcCardPointer.SetCardSlot(_dstCardPointer.cardSlotRect);
        if (srcIndex < dstIndex)
        {
            for (int i = srcIndex + 1; i <= dstIndex; i++)
            {
                Card card = GetCard(_cardSlots[i]);
                card.GetMove(_cardSlots[i - 1]);
                _cardsDic[card.cardSlotRect] = card;
            }
        }
        else
        {
            for (int i = srcIndex - 1; i >= dstIndex; i--)
            {
                Card card = GetCard(_cardSlots[i]);
                card.GetMove(_cardSlots[i + 1]);
                _cardsDic[card.cardSlotRect] = card;
            }
        }

        _cardsDic[_srcCardPointer.cardSlotRect] = _srcCardPointer;

        _isSwap = false;
    }

    public void ChooseCard(Card card)
    {
        if (!CanChooseCard())
        {
            Debug.Log("The chosen card number is out of bound.");
            return;
        }
        _chosenCards.Add(card);
    }
    public bool CanChooseCard()
    {
        return _chosenCards.Count < gameConfigSO.maxCard2Play;
    }
    public void RejectCard(Card card)
    {
        for (int i = 0; i < _chosenCards.Count; i++)
        {
            if (_chosenCards[i] == card)
            {
                _chosenCards.RemoveAt(i);
            }
        }
    }
    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.CanInteract(true);
        foreach (RectTransform slot in _cardSlots)
        {
            if (_cardsDic[slot] == null)
            {
                slot.gameObject.SetActive(true);
                _cardsDic[slot] = card;
                card.GetMove(slot);
                _ = card.FaceCardUp();
                curCardNum++;
                return;
            }
        }
    }
}
