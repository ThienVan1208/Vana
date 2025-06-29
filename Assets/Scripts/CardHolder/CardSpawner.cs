using System;
using System.Collections.Generic;
using UnityEngine;
public static class CardSpawnerEvent
{
    public static Func<Card> GetCardEvent;
    public static Card RaiseGetCardEvent()
    {
        return GetCardEvent?.Invoke();
    }
}
public class CardSpawner : MonoBehaviour
{
    private int _initNum = 20;
    [SerializeField] private AllCardContainerSO _allCardsContainerSO;
    private Stack<Card> _unusedCardList = new Stack<Card>();
    [SerializeField] private RectTransform _cardSpawnerPos;
    private void Awake()
    {
        Init();
    }
    // private void OnDestroy() {
    //     _allCardsContainerSO.Init();
    //     _unusedCardList.Clear();
    // }
    private void OnEnable()
    {
        CardSpawnerEvent.GetCardEvent += GetCards;
    }
    private void OnDisable()
    {
        CardSpawnerEvent.GetCardEvent -= GetCards;
    }
    private void Init()
    {
        _allCardsContainerSO.Init();
        for (int i = 0; i < _initNum; i++)
        {
            GameObject cardObj = Instantiate(_allCardsContainerSO.GetRandomCardPrefab(), _cardSpawnerPos.position, Quaternion.identity);
            cardObj.SetActive(false);
            cardObj.transform.SetParent(_cardSpawnerPos, false);
            _unusedCardList.Push(cardObj.GetComponent<Card>());
        }
    }
    public Card GetCards()
    {
        if (_unusedCardList.Count <= 0)
        {
            GameObject cardObj = Instantiate(_allCardsContainerSO.GetRandomCardPrefab(), _cardSpawnerPos.position, Quaternion.identity);
            cardObj.SetActive(false);
            cardObj.transform.SetParent(_cardSpawnerPos, false);
            _unusedCardList.Push(cardObj.GetComponent<Card>());
        }
        
        return _unusedCardList.Pop();
    }
}
