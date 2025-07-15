using System.Collections.Generic;


public class Ver1PlayerAI : CardPlayAI
{
    private List<Card> cards = new List<Card>();
    public override List<Card> GetCardPlayingAI()
    {
        return cards;
    }

}
