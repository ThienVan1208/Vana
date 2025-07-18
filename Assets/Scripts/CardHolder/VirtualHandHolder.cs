

public abstract class VirtualHandHolder : PlayableCardHolder
{

    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.CanInteract(false);
        card.GetIdleEffect();
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

        CheckEndGameConditionEvent.RaiseEvent();
    }

    public abstract override bool HelpPlayingCard();

    
}
