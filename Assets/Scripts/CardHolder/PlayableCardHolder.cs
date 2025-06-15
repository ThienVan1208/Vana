using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlayableCardHolder : CardHolder, IHelpPlayingCard
{
    // Used to assign chosen card list in RuleGameHandle.
    [SerializeField] protected ChosenCardEventSO chosenCardEventSO;

    public abstract bool HelpPlayingCard();
    public virtual void RelocateCards()
    {
        var slots = _cardsDic.Keys.ToList();
        var cards = new List<Card>();
        foreach (var slot in slots)
        {
            if (slot.gameObject.activeSelf && _cardsDic[slot] != null) cards.Add(_cardsDic[slot]);
        }
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardSlotRect == slots[i]) continue;

            _cardsDic[cards[i].cardSlotRect] = null;
            _cardsDic[slots[i]] = cards[i];
            cards[i].GetMove(slots[i]);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].gameObject.activeSelf) break;
            if ( i < gameConfigSO.initCardNum) continue;

            if(_cardsDic[slots[i]] == null) slots[i].gameObject.SetActive(false);
        }
    }
}
