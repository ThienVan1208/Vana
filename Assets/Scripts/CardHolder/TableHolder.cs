using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TableHolder : CardHolder
{
    protected override void InitHolder()
    {
        base.InitHolder();

        foreach (var slot in _cardSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }
    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.CanInteract(false);
        foreach (RectTransform slot in _cardSlots)
        {
            if (_cardsDic[slot] == null)
            {
                slot.gameObject.SetActive(true);
                _cardsDic[slot] = card;
                card.myRect.SetParent(slot, false);
                card.GetMove(slot);
                return;
            }
        }
    }
    public override void DisconnectCardSlot(Card card)
    {
        RectTransform slot = GetCardSlot(card);
        if (slot != null)
        {
            slot.gameObject.SetActive(false);
            _cardsDic[slot] = null;
        }
    }
    
}
