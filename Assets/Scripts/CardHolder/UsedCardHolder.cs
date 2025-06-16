using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UsedCardHolder : MonoBehaviour
{
    private Queue<Card> _usedCardQueue = new Queue<Card>();
    [SerializeField] private RectTransform _usedCardContainer;

    public async UniTask AddUsedCards(List<Card> usedCards)
    {

        for (int i = 0; i < usedCards.Count; i++)
        {
            // Pass the cancellation token to FaceCardDown
            await usedCards[i].FaceCardDown();
            usedCards[i].GetMove(_usedCardContainer); // Check if this needs to be async
            _usedCardQueue.Enqueue(usedCards[i]);
            usedCards[i].CanInteract(false);
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
