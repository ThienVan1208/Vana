using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HandHolder : PlayableCardHolder
{
    [SerializeField] private List<Card> _chosenCards = new List<Card>();
    [SerializeField] private IntEventSO _exchangeCardEventSO;
    private Card _srcCardPointer;
    private Card _dstCardPointer;
    private bool _isDrag = false;
    private bool _isSwap = false;
    private int _changeCardNum = 0;
    protected override void OnDestroy()
    {
        _chosenCards.Clear();
    }
    private void OnEnable() {
        _exchangeCardEventSO.RaiseEvent(_changeCardNum);
    }

    #region Add card
    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.CanInteract(true);
        foreach (var keyVal in _cardsDic)
        {
            if (_cardsDic[keyVal.Key] == null)
            {
                keyVal.Key.gameObject.SetActive(true);
                _cardsDic[keyVal.Key] = card;
                curCardNum++;
                card.GetMove(keyVal.Key);
                _ = card.FaceCardUp();
                return;
            }
        }
    }
    #endregion

    #region Play card
    // Used thru button.
    public override bool HelpPlayingCard()
    {
        if (_chosenCards.Count < gameConfigSO.minCard2Play)
        {
            Debug.Log("Must choose at least" + gameConfigSO.minCard2Play + "cards");
            return false;
        }

        foreach (var card in _chosenCards)
        {
            _cardsDic[card.transform.parent as RectTransform] = null;
            curCardNum--;
        }

        chosenCardEventSO.RaiseEvent(_chosenCards);

        ObjectPoolManager.GetPoolingObject<CardPSEffect>()?.StopGlowEffect(isInactive: true);

        // Clear chosen card list for the next choosing turn.
        _chosenCards.Clear();

        return true;
    }
    #endregion

    #region Change card
    public async UniTask<bool> HelpChangingCard()
    {
        if (_changeCardNum <= 0)
        {
            Debug.Log("No more exchange");
            return false;
        }

        foreach (var card in _chosenCards)
        {
            _cardsDic[card.transform.parent as RectTransform] = null;
            curCardNum--;
        }

        AddChangeCardNum(-1);
        
        ObjectPoolManager.GetPoolingObject<CardPSEffect>()?.StopGlowEffect(isInactive: true);

        // Move cards to cardSpawner.
        await CardSpawnerEvent.RaiseAddCardEvent(_chosenCards);

        // Get new cards.
        for (int i = 0; i < _chosenCards.Count; i++)
        {
            Card newCard = CardSpawnerEvent.GetCardEvent();
            newCard.gameObject.SetActive(true);
            AddCard(newCard);
        }

        _chosenCards.Clear();
        return true;
    }
    public void AddChangeCardNum(int val = 1)
    {
        if (_changeCardNum >= 3 && val >= 0) return;
        _changeCardNum += val;
        _exchangeCardEventSO.RaiseEvent(_changeCardNum);
    }
    #endregion

    #region  Swap card
    public void SetSrcCardPointer(Card card)
    {
        if (!_isDrag) return;
        _srcCardPointer = card;
    }
    public void SetDstCardPointer(Card card)
    {
        if (!_isDrag) return;

        _dstCardPointer = card;
        SwapCard();
    }

    public void SwapCard()
    {
        if (_isSwap) return;

        _isSwap = true;
        int srcIndex = GetIndexOfCardSlot(_srcCardPointer.cardSlotRect);
        int dstIndex = GetIndexOfCardSlot(_dstCardPointer.cardSlotRect);
        if (srcIndex < 0 || dstIndex < 0)
        {
            Debug.LogWarning("Get index of slot is null");
            return;
        }

        _srcCardPointer.SetCardSlot(_dstCardPointer.cardSlotRect);
        if (srcIndex < dstIndex)
        {
            for (int i = srcIndex + 1; i <= dstIndex; i++)
            {
                Card card = GetCard(i);
                var newSlot = GetCardSlot(i - 1);
                if (card == null)
                {
                    Debug.LogWarning("card is null");
                    return;
                }
                if (newSlot == null)
                {
                    Debug.LogWarning("new slot is null");
                }
                card.GetMove(newSlot);
                _cardsDic[newSlot] = card;
            }
        }
        else
        {
            for (int i = srcIndex - 1; i >= dstIndex; i--)
            {
                Card card = GetCard(i);
                card.GetMove(GetCardSlot(i + 1));
                _cardsDic[card.cardSlotRect] = card;
            }
        }

        _cardsDic[_srcCardPointer.cardSlotRect] = _srcCardPointer;

        _isSwap = false;
    }
    #endregion

    #region Check drag
    public bool IsDrag()
    {
        return _isDrag;
    }
    public void SetDrag(bool isDrag)
    {
        _isDrag = isDrag;
    }
    #endregion

    #region  Choose card
    public void ChooseCard(Card card)
    {
        if (!CanChooseCard())
        {
            Debug.Log("The chosen card number is out of bound.");
            return;
        }
        _chosenCards.Add(card);
        if (_chosenCards.Count == 1) ObjectPoolManager.GetPoolingObject<CardPSEffect>()?.GetGlowEffect(_chosenCards[0].frontImg.transform);
    }
    public bool CanChooseCard()
    {
        return _chosenCards.Count < gameConfigSO.maxCard2Play;
    }
    #endregion

    #region Reject card
    public void RejectCard(Card card)
    {
        for (int i = 0; i < _chosenCards.Count; i++)
        {
            if (_chosenCards[i] == card)
            {
                if (i == 0)
                {
                    ObjectPoolManager.GetPoolingObject<CardPSEffect>()?.StopGlowEffect(isInactive: true);
                    if (_chosenCards.Count > 1)
                    {
                        ObjectPoolManager.GetPoolingObject<CardPSEffect>()?.GetGlowEffect(_chosenCards[1].frontImg.transform);

                    }
                }
                _chosenCards.RemoveAt(i);
                return;
            }
        }
    }


    #endregion

}
