using System.Collections.Generic;
using UnityEngine;

public class VirtualHandHolder : PlayableCardHolder
{
    private CardPlayAI _cardPlayingAI;
    protected override void Awake()
    {
        base.Awake();
        _cardPlayingAI = GetComponent<CardPlayAI>();
        if (_cardPlayingAI == null)
        {
            Debug.LogError("Card Playing AI is null.");
        }
        _cardPlayingAI.SetVirtualHandHolder(this);
    }

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

    public override bool HelpPlayingCard()
    {
        var cards = _cardPlayingAI.GetCardPlayingAI();
        if(cards.Count == 0) return false;

        chosenCardEventSO.RaiseEvent(cards);
        return true;
    }
}
