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
                // slot.gameObject.SetActive(true);
                _cardsDic[slot] = card;

                card.GetMove(slot);

                return;
            }
        }
    }
    public void ActiveSlotBeforeAddCard(int numSlot)
    {
        for (int i = 0; i < numSlot; i++)
        {
            _cardSlots[i].gameObject.SetActive(true);
        }
    }
    public void RefreshTable()
    {
        foreach (RectTransform slot in _cardSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }
    
}
