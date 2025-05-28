using System.Collections.Generic;
using UnityEngine;

public class UsedCardHolder : MonoBehaviour
{
    private Queue<Card> _usedCardQueue = new Queue<Card>();
    [SerializeField] private RectTransform _usedCardContainer;

    public void AddUsedCards(List<Card> usedCards)
    {
        for (int i = 0; i < usedCards.Count; i++) {
            usedCards[i].FaceCardDown();
            usedCards[i].GetMove(_usedCardContainer);
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
