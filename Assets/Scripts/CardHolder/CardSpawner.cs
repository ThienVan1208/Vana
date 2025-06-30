using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
public static class CardSpawnerEvent
{
    public static Func<Card> GetCardEvent;
    public static Card RaiseGetCardEvent()
    {
        return GetCardEvent?.Invoke();
    }

    public static Func<List<Card>, UniTask> AddCardEvent;
    public static async UniTask RaiseAddCardEvent(List<Card> cards)
    {
        await (AddCardEvent?.Invoke(cards) ?? UniTask.CompletedTask);
    }
}
public class CardSpawner : MonoBehaviour
{
    private int _initNum = 52;
    [SerializeField] private AllCardContainerSO _allCardsContainerSO;
    private Queue<Card> _unusedCardList = new Queue<Card>();
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
        CardSpawnerEvent.AddCardEvent += AddCards;
    }
    private void OnDisable()
    {
        CardSpawnerEvent.GetCardEvent -= GetCards;
        CardSpawnerEvent.AddCardEvent -= AddCards;
    }
    private void Init()
    {
        _allCardsContainerSO.Init();
        for (int i = 0; i < _initNum; i++)
        {
            GameObject cardObj = Instantiate(_allCardsContainerSO.GetRandomCardPrefab(), _cardSpawnerPos.position, Quaternion.identity);
            cardObj.SetActive(false);
            cardObj.transform.SetParent(_cardSpawnerPos, false);
            _unusedCardList.Enqueue(cardObj.GetComponent<Card>());
        }
    }
    public Card GetCards()
    {
        if (_unusedCardList.Count <= 0)
        {
            GameObject cardObj = Instantiate(_allCardsContainerSO.GetRandomCardPrefab(), _cardSpawnerPos.position, Quaternion.identity);
            cardObj.SetActive(false);
            cardObj.transform.SetParent(_cardSpawnerPos, false);
            _unusedCardList.Enqueue(cardObj.GetComponent<Card>());
        }

        return _unusedCardList.Dequeue();
    }

    public async UniTask AddCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            await card.FaceCardDown();
            card.GetMove(_cardSpawnerPos);
            _unusedCardList.Enqueue(card);
            card.CanInteract(false);
            await UniTask.WaitUntil(
                () => Vector2.Distance(card.myRect.position, _cardSpawnerPos.position) <= 0.01f
            );
            card.gameObject.SetActive(false);
        }
        await UniTask.WaitForEndOfFrame();
    }
}
