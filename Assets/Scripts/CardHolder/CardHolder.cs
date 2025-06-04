using System.Collections.Generic;
using UnityEngine;
public class CardHolder : MonoBehaviour
{
    [SerializeField] protected GameConfigSO gameConfigSO;

    // This contains cardSlot obj.
    [SerializeField] protected List<RectTransform> _cardSlots = new List<RectTransform>();

    // The key is cardSlot, value is card.
    protected Dictionary<RectTransform, Card> _cardsDic = new Dictionary<RectTransform, Card>();

    protected int curCardNum = 0;

    protected virtual void Awake()
    {
        InitHolder();
    }
    protected virtual void InitHolder()
    {
        foreach (var slot in _cardSlots)
        {
            _cardsDic[slot] = null;
        }
    }
    protected int GetCardNum()
    {
        return curCardNum;
    }
    public virtual List<RectTransform> GetCardSlots()
    {
        return _cardSlots;
    }
    public virtual RectTransform GetCardSlot(Card card)
    {
        foreach (var slot in _cardSlots)
        {
            if (slot == card.cardSlotRect)
            {
                return slot;
            }
        }
        return null;
    }
    public virtual Card GetCard(RectTransform cardSlot, bool disconnect = false)
    {

        if (_cardsDic.TryGetValue(cardSlot, out Card card) == true)
        {
            if (disconnect) _cardsDic[cardSlot] = null;
            return card;
        }
        else
        {
            Debug.LogWarning("Get card is null");
            return null;
        }
    }
    public virtual Card GetCard(int index = -1, bool disconnect = false)
    {
        if (index == -1)
        {
            foreach (KeyValuePair<RectTransform, Card> keyVal in _cardsDic)
            {
                if (_cardsDic[keyVal.Key] != null)
                {
                    if (disconnect) _cardsDic[keyVal.Key] = null;
                    return keyVal.Value;
                }
            }
            Debug.Log("Get card is null");
            return null;
        }
        else
        {
            if (_cardsDic.TryGetValue(_cardSlots[index], out Card card) == true)
            {
                if (disconnect) _cardsDic[_cardSlots[index]] = null;
                return card;
            }
            else
            {
                Debug.LogWarning("Get card is null");
                return null;
            }
        }
    }

    public virtual int GetIndexOfCardSlot(RectTransform cardSlot)
    {
        for (int i = 0; i < _cardSlots.Count; i++)
        {
            if (cardSlot.position.x == _cardSlots[i].position.x) return i;
        }
        return -1;
    }

    public virtual void AddCard(Card card)
    {
        // Remove from previous cardHolder.
        DisconnectCardSlot(card);

        // Set new cardHolder.
        card.SetCardHolder(this);
        // Debug.Log("Connect to " + card.cardHolder.gameObject.name);
    }
    
    // Used to disconnect card from previous cardholder.
    public virtual void DisconnectCardSlot(Card card)
    {
        if (card.cardHolder == null || card.cardSlotRect == null) return;

        // Debug.Log("Disconnect from " + card.cardHolder.gameObject.name);
        card.cardHolder.SetCardDic(card.cardSlotRect);

    }
    protected virtual void SetCardDic(RectTransform slot, Card card = null)
    {
        _cardsDic[slot] = card;
    }


}
