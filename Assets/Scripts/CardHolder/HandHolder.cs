using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandHolder : CardHolder
{
    [SerializeField] private ChosenCardEventSO _chosenCardEventSO;
    [SerializeField]private List<Card> _chosenCards = new List<Card>();
    private Card _srcCardPointer;
    private Card _dstCardPointer;
    private bool _isDrag = false;
    private bool _isSwap = false;
    
    // Used thru button.
    public void PlayCard(){
        _chosenCardEventSO.RaiseEvent(_chosenCards);
    }
    public void SetSrcCardPointer(Card card){
        if(!_isDrag) return;
        _srcCardPointer = card;
    }
    public void SetDstCardPointer(Card card){
        if(!_isDrag) return;
        
        _dstCardPointer = card;
        SwapCard();
    }
    public bool IsDrag(){
        return _isDrag;
    }
    public void SetDrag(bool isDrag){
        _isDrag = isDrag;
    }
    protected override void InitHolder()
    {
        base.InitHolder();
    }
    protected override void AddCard(Card card)
    {
        base.AddCard(card);
    }
    public void UpdateCardVsSlot(){
        foreach(var cardSlotPair in _cardsDic){
            RectTransform slot = cardSlotPair.Key;
            Card card = cardSlotPair.Value;
            //card.GetMove(slot);
            (card.transform as RectTransform).position = card.cardSlotRect.position;
            //print("slot: " + slot.name + "card: " + card.name);
        }
    }
    public void SwapCard(){
        if(_isSwap) return;

        _isSwap = true;
        int srcIndex = GetIndexOfCardSlot(_srcCardPointer.cardSlotRect);
        int dstIndex = GetIndexOfCardSlot(_dstCardPointer.cardSlotRect);
        //GetCard(_srcCardPointer.cardSlotRect).SetCardSlot(_dstCardPointer.cardSlotRect);
        _srcCardPointer.SetCardSlot(_dstCardPointer.cardSlotRect);
        if(srcIndex < dstIndex){
            for(int i = srcIndex + 1; i <= dstIndex; i++){
                Card card = GetCard(_cardSlots[i]);
                card.GetMove(_cardSlots[i - 1]);
                _cardsDic[card.cardSlotRect] = card;
            }
        }
        else{
            for(int i = srcIndex - 1; i >= dstIndex; i--){
                Card card = GetCard(_cardSlots[i]);
                card.GetMove(_cardSlots[i + 1]);
                _cardsDic[card.cardSlotRect] = card;
            }
        }
        
        _cardsDic[_srcCardPointer.cardSlotRect] = _srcCardPointer;

        _isSwap = false;
    }

    public void ChooseCard(Card card){
        if(!CanChooseCard()){
            Debug.Log("The chosen card number is out of bound.");
            return;
        }
        _chosenCards.Add(card);
    }
    public bool CanChooseCard(){
        return _chosenCards.Count < 4;
    }
    public void RejectCard(Card card){
        for(int i = 0; i < _chosenCards.Count; i++){
            if(_chosenCards[i] == card){
                for(int j = i; j < _chosenCards.Count - 1; j++){
                    _chosenCards[j] = _chosenCards[j + 1];
                }
                break;
            }
        }
        _chosenCards[_chosenCards.Count - 1] = null;
    }
}
