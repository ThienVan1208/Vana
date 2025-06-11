using System.Collections.Generic;
using UnityEngine;

public class VirtualHandHolder : PlayableCardHolder
{
    public override void AddCard(Card card)
    {
        // DisconnectCardSlot(card);
        base.AddCard(card);
        card.CanInteract(false);
        foreach (var keyVal in _cardsDic)
        {
            if (_cardsDic[keyVal.Key] == null)
            {
                curCardNum++;
                keyVal.Key.gameObject.SetActive(true);
                _cardsDic[keyVal.Key] = card;
                card.GetMove(keyVal.Key);
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
            if (newCard == null)
            {
                i--;
                Debug.LogWarning("Card " + i + " is null");
                continue;
            }
            cards.Add(newCard);
            curCardNum--;
            // if (curCardNum > gameConfigSO.initCardNum)
            // {
            //     newCard.cardSlotRect?.gameObject.SetActive(false);
            // }
        }
        chosenCardEventSO.RaiseEvent(cards);

        return true;
    }
}
