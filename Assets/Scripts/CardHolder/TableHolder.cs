using System.Linq;
using UnityEngine;

public class TableHolder : CardHolder
{
    protected override void InitHolder()
    {
        base.InitHolder();

        foreach (var keyVal in _cardsDic)
        {
            keyVal.Key.gameObject.SetActive(false);
        }
    }
    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.CanInteract(false);

        foreach (var keyVal in _cardsDic)
        {
            if (_cardsDic[keyVal.Key] == null)
            {
                _cardsDic[keyVal.Key] = card;
                card.GetMove(keyVal.Key);
                return;
            }
            else
            {
                Debug.LogWarning("can not add to table");
            }
        }
    }

    
    // Used to active slot before using AddCard method.
    public void ActiveSlotBeforeAddCard(int numSlot)
    {
        foreach (var keyVal in _cardsDic)
        {
            keyVal.Key.gameObject.SetActive(true);
            numSlot--;
            if (numSlot <= 0) return;
        }
    }

    // Used to reset _cardsDic and card slot when moving them to used card holder.
    public void RefreshTable()
    {
        var keys = _cardsDic.Keys.ToList(); // Create a copy of the keys.
        foreach (var key in keys)
        {
            _cardsDic[key] = null; // Safe to modify.
            key.gameObject.SetActive(false);
        }
    }
}
