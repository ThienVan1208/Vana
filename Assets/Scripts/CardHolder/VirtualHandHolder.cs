using System.Collections.Generic;
using UnityEngine;

public class VirtualHandHolder : PlayableCardHolder
{
    public override void AddCard(Card card)
    {
        DisconnectCardSlot(card);
        base.AddCard(card);
        card.CanInteract(false);
        foreach (RectTransform slot in _cardSlots)
        {
            if (_cardsDic[slot] == null)
            {
                curCardNum++;
                slot.gameObject.SetActive(true);
                _cardsDic[slot] = card;
                card.GetMove(slot);
                return;
            }
        }
    }

    public override bool HelpPlayingCard()
    {
        List<Card> cards = new List<Card>();
        int ranNum = Mathf.Min(Random.Range(gameConfigSO.minCard2Play, gameConfigSO.maxCard2Play + 1), curCardNum);
        
        for (int i = 0; i < ranNum; i++)
        {
            Card newCard = GetCard(disconnect: true);
            cards.Add(newCard);
            curCardNum--;
            if (curCardNum > gameConfigSO.initCardNum)
            {
                GetCardSlot(newCard)?.gameObject.SetActive(false);
            }
        }
        chosenCardEventSO.RaiseEvent(cards);
        return true;
    }
}
