using System.Collections.Generic;
using UnityEngine;

public class Ver0PlayerAI : CardPlayAI
{
    public override List<Card> GetCardPlayingAI()
    {
        List<Card> cards = new List<Card>();
        int ranNum = Mathf.Min(Random.Range(GameConfiguration.minCard2Play, GameConfiguration.maxCard2Play + 1)
                            , virtualHandHolder.curCardNum);

        for (int i = 0; i < ranNum; i++)
        {
            Card newCard = virtualHandHolder.GetCard(disconnect: true);
            if (newCard == null)
            {
                i--;
                Debug.LogWarning("Card " + i + " is null");
                continue;
            }
            cards.Add(newCard);
            virtualHandHolder.curCardNum--;
        }

        return cards;
    }
}
