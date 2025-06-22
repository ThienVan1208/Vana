using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CardHolder : MonoBehaviour
{
    [SerializeField] protected GameConfigSO gameConfigSO;
    public List<RectTransform> testList = new List<RectTransform>();

    // The key is cardSlot, value is card.
    protected Dictionary<RectTransform, Card> _cardsDic = new Dictionary<RectTransform, Card>();

    public int curCardNum { get;  protected set; }

    protected virtual void Awake()
    {
        InitHolder();
        // Debug.Log("awake in card holder");
    }
    protected virtual void OnDestroy()
    {
        // InitHolder();
        // Debug.Log("destroy in card holder");
    }
    protected virtual void InitHolder()
    {
        curCardNum = 0;
        foreach (Transform child in transform)
        {
            _cardsDic[child as RectTransform] = null;
            testList.Add(child as RectTransform);
        }
    }

    #region Get card
    // public int GetCardNum()
    // {
    //     return curCardNum;
    // }

    public virtual RectTransform GetCardSlot(int index = 0)
    {
        return _cardsDic.ElementAt(index).Key;
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
            foreach (var key in _cardsDic.Keys.ToList())
            {
                if (_cardsDic[key] != null)
                {
                    Card retCard = _cardsDic[key];
                    if (disconnect) _cardsDic[key] = null;
                    return retCard;
                }
            }
            return null;
        }
        else
        {
            if (_cardsDic.Count > index)
            {
                KeyValuePair<RectTransform, Card> keyVal = _cardsDic.ElementAt(index);
                Card retCard = keyVal.Value; 
                if (disconnect) _cardsDic[keyVal.Key] = null;
                return retCard;
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
        int index = 0;
        foreach (KeyValuePair<RectTransform, Card> keyVal in _cardsDic)
        {
            if (keyVal.Key == cardSlot)
            {
                return index;
            }

            index++;
        }

        return -1;
    }
    #endregion

    #region Add card
    public virtual void AddCard(Card card)
    {
        if (card == null) return;

        // Remove from previous cardHolder.
        DisconnectCardSlot(card);

        // Set new cardHolder.
        card.SetCardHolder(this);
    }

    // Used to disconnect card from previous cardholder.
    public virtual void DisconnectCardSlot(Card card)
    {
        if (card.cardHolder == null || card.cardSlotRect == null) return;

        card.cardHolder.SetCardDic(card.cardSlotRect, null);
    }
    protected virtual void SetCardDic(RectTransform slot, Card card = null)
    {
        _cardsDic[slot] = card;
    }
    #endregion

}
