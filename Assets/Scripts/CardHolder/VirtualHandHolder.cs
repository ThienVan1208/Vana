using UnityEngine;

public class VirtualHandHolder : CardHolder
{
    public override void AddCard(Card card)
    {
        card.CanInteract(false);
        foreach (RectTransform slot in _cardSlots)
        {
            if (_cardsDic[slot] == null)
            {
                _cardsDic[slot] = card;
                card.GetMove(slot);
                card.myRect.SetParent(slot, false);
                return;
            }
        }
    }
}
