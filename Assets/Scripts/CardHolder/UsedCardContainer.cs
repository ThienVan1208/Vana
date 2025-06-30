using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UsedCardContainerEvent
{
    public static Func<List<Card>> GetUsedCardListEvent;
    public static List<Card> RaiseGetUsedCardListEvent()
    {
        return GetUsedCardListEvent?.Invoke();
    }

    public static Func<List<Card>, UniTask> AddUsedCardEvent;
    public static async UniTask RaiseAddUsedCardEvent(List<Card> cards)
    {
        await (AddUsedCardEvent?.Invoke(cards) ?? UniTask.CompletedTask);
    }
} 
public class UsedCardContainer : MonoBehaviour
{
    private Queue<Card> _usedCardQueue = new Queue<Card>();
    [SerializeField] private RectTransform _usedCardContainer;
    private void OnEnable()
    {
        UsedCardContainerEvent.GetUsedCardListEvent += GetUsedCardList;
        UsedCardContainerEvent.AddUsedCardEvent += AddUsedCards;
    }
    private void OnDisable() {
        UsedCardContainerEvent.GetUsedCardListEvent += GetUsedCardList;
        UsedCardContainerEvent.AddUsedCardEvent += AddUsedCards;
    }
    public async UniTask AddUsedCards(List<Card> usedCards)
    {

        foreach (Card card in usedCards)
        {
            await card.FaceCardDown();
            card.GetMove(_usedCardContainer);
            _usedCardQueue.Enqueue(card);
            card.CanInteract(false);
        }

    }

    public List<Card> GetUsedCardList()
    {
        List<Card> retList = new List<Card>();
        while (_usedCardQueue.Count != 0)
        {
            retList.Add(_usedCardQueue.Dequeue());
        }
        return retList;
    }

}
