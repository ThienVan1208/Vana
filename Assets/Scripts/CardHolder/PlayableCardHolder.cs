using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlayableCardHolder : CardHolder, IHelpPlayingCard
{
    // Used to assign chosen card list in RuleGameHandle.
    [SerializeField] protected ChosenCardEventSO chosenCardEventSO;

    // Ref in RuleGameHandler, PlayerBase.
    [SerializeField] protected RetBoolEventSO checkEndGameEventSO;

    public abstract bool HelpPlayingCard();
    public virtual void RelocateCards()
    {
        
        var slots = _cardsDic.Keys.ToList();
        var cards = new List<Card>();

        //  Get current card list.
        foreach (var slot in slots)
        {
            if (slot.gameObject.activeSelf && _cardsDic[slot] != null) cards.Add(_cardsDic[slot]);
        }

        // Relocate cards to the slots.
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardSlotRect == slots[i]) continue;

            _cardsDic[cards[i].cardSlotRect] = null;
            _cardsDic[slots[i]] = cards[i];
            cards[i].GetMove(slots[i], getAudio: false);
        }

        // Hide the slots that are not used.
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].gameObject.activeSelf) break;
            if (i < GameConfiguration.initCardNum) continue;

            if (_cardsDic[slots[i]] == null) slots[i].gameObject.SetActive(false);
        }
    }
}
