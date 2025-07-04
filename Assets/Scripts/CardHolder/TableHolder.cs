using System.Linq;
using UnityEngine;

public class TableHolder : CardHolder
{
    [SerializeField] private IntEventSO _activeCardSlotEventSO;
    [SerializeField] private CardEventSO _moveCardToTableEventSO;
    [SerializeField] private VoidEventSO _refeshTableEventSO;
    private void OnEnable()
    {
        _activeCardSlotEventSO.EventChannel += ActiveSlotBeforeAddCard;
        _moveCardToTableEventSO.EventChannel += AddCard;
        _refeshTableEventSO.EventChannel += RefreshTable;
    }
    private void OnDisable()
    {
        _activeCardSlotEventSO.EventChannel -= ActiveSlotBeforeAddCard;
        _moveCardToTableEventSO.EventChannel -= AddCard;
        _refeshTableEventSO.EventChannel -= RefreshTable;
    }
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
        card.GetIdleEffect(false);
        foreach (var keyVal in _cardsDic)
        {
            if (_cardsDic[keyVal.Key] == null)
            {
                _cardsDic[keyVal.Key] = card;
                card.GetMove(keyVal.Key);
                curCardNum++;
                return;
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
        curCardNum = 0;
        var keys = _cardsDic.Keys.ToList(); // Create a copy of the keys.
        foreach (var key in keys)
        {
            _cardsDic[key] = null; // Safe to modify.
            key.gameObject.SetActive(false);
        }
    }
    protected override void SetCardDic(RectTransform slot, Card card = null)
    {
        base.SetCardDic(slot, card);
        curCardNum--;
    }
}
