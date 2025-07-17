using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


public class Ver1PlayerAI : CardPlayAI
{
    [Range(0, 100)]
    public float drawCardPercent;
    private List<Card> _cards = new List<Card>();
    private List<Card> _playCards = new List<Card>();
    private NativeList<int> _cardRankList;
    private NativeList<int> _idxList;
    JobHandle jobHandle;
    private void Awake()
    {
        _cardRankList = new NativeList<int>(allocator: Allocator.Persistent);
        _idxList = new NativeList<int>(allocator: Allocator.Persistent);
    }
    private void OnDestroy()
    {
        _cardRankList.Dispose();
        _idxList.Dispose();
    }
    public override List<Card> GetCardPlayingAI()
    {
        _cardRankList.Clear();
        _idxList.Clear();
        _cards.Clear();
        _playCards.Clear();

        _cards = virtualHandHolder.GetCardList();
        foreach (var card in _cards)
        {
            _cardRankList.Add((int)card.GetCardRank());
        }
        FindSameRankCardJob rankJob = new FindSameRankCardJob
        {
            cardRankList = _cardRankList,
            idxList = _idxList
        };
        jobHandle = rankJob.Schedule();
        jobHandle.Complete();

        if (_idxList.Length == 0)
        {
            // Draw card handle.
            int ranNum = Mathf.Min(Random.Range(GameConfiguration.minCard2Play, GameConfiguration.maxCard2Play + 1)
                                , virtualHandHolder.curCardNum);

            for (int i = 0; i < ranNum; i++)
            {
                Card newCard = virtualHandHolder.GetCard(disconnect: true);
                if (newCard == null)
                {
                    i--;
                    Debug.LogWarning("Card " + i + " is null");
                    continue;
                }
                _playCards.Add(newCard);
                virtualHandHolder.curCardNum--;
            }

            return _playCards;
        }
        else
        {
            if (Utils.GetPercent(drawCardPercent))
            {
                // Draw card handle.
                foreach (var idx in _idxList)
                {
                    _playCards.Add(_cards[idx]);
                    virtualHandHolder.curCardNum--;
                }
            }
            else
            {
                foreach (var idx in _idxList)
                {
                    _playCards.Add(_cards[idx]);
                    virtualHandHolder.curCardNum--;
                }
            }
        }

        return _playCards;
    }

}
struct FindSameRankCardJob : IJob
{
    public NativeList<int> cardRankList;
    public NativeList<int> idxList;
    public void Execute()
    {
        for (int i = 0; i < cardRankList.Length; i++)
        {
            for (int j = 0; j < cardRankList.Length; j++)
            {
                if (i != j && cardRankList[i] == cardRankList[j])
                {
                    idxList.Add(j);
                }
            }

            if (idxList.Length != 0)
            {
                idxList.Add(i);
                return;
            }
        }
    }
}
