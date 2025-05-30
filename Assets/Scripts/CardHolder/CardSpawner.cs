using System.Collections.Generic;
using UnityEngine;

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
        return _unusedCardList.Pop();
    }
}
