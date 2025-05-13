using System.Collections.Generic;
using UnityEngine;
public class CardHolder : MonoBehaviour
{
    public int MAX_CARD_SLOT { get; protected set; }

    // This contains cardSlot obj.
    [SerializeField] protected List<RectTransform> _cardSlots = new List<RectTransform>();

    // The key is cardSlot, value is card.
    protected Dictionary<RectTransform, Card> _cardsDic = new Dictionary<RectTransform, Card>();

    protected virtual void Awake()
    {
        InitHolder();
    }
    protected virtual void InitHolder()
    {
        foreach (var slot in _cardSlots)
        {
            if (!slot.gameObject.activeSelf) continue;

            if (slot.GetChild(0).TryGetComponent<Card>(out var card) == true)
            {
                _cardsDic[slot] = card;
                card.SetCardHolder(this);
            }
            else
            {
                _cardsDic[slot] = null;
            }
        }
    }
    public virtual List<RectTransform> GetCardSlots()
    {
        return _cardSlots;
    }
    public virtual Card GetCard(RectTransform cardSlot)
    {
        //return _cardsDic[cardSlot];
        if (_cardsDic.TryGetValue(cardSlot, out Card card) == true)
        {
            return card;
        }
        else
        {
            Debug.Log("get card is null");
            return null;
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
    protected virtual void AddCard(Card card) { }
    protected virtual void AddCard(Queue<Card> cardList) { }
    protected virtual void RemoveCard(Card card) { }
    protected virtual void RemoveCard(Queue<Card> cardList) { }


}
