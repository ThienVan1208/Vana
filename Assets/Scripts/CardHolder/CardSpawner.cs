using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    private int _initNum = 30;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Stack<Card> _unusedCardList = new Stack<Card>();
    [SerializeField] private RectTransform _cardSpawnerPos;
    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        for (int i = 0; i < _initNum; i++)
        {
            GameObject cardObj = Instantiate(_cardPrefab, _cardSpawnerPos.position, Quaternion.identity);
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
