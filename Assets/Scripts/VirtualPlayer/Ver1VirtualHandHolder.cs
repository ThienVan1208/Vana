using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

[BurstCompile]
struct FindSameRankCardJob : IJob
{
    public NativeList<int> cardRankList;
    public NativeList<int> idxList;
    public void Execute()
    {
        for (int i = 0; i < cardRankList.Length; i++)
        {
            for (int j = i + 1; j < cardRankList.Length; j++)
            {
                if (cardRankList[i] == cardRankList[j])
                {
                    idxList.Add(i);
                    idxList.Add(j);
                    return;
                }
            }
        }
    }
}
[BurstCompile]
public class Ver1VirtualHandHolder : Ver0VirtualHandHolder
{
    [Range(0, 100)]
    public float playExtraCardPercent;
    private List<Card> _cards = new List<Card>();
    private List<Card> _playCards = new List<Card>();
    private NativeList<int> _cardRankList;
    private NativeList<int> _idxList;
    JobHandle jobHandle;

    protected override void Awake()
    {
        base.Awake();
        _cardRankList = new NativeList<int>(allocator: Allocator.Persistent);
        _idxList = new NativeList<int>(allocator: Allocator.Persistent);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _cardRankList.Dispose();
        _idxList.Dispose();
    }

    protected override async void GetCardPlayingAI()
    {
        _cardRankList.Clear();
        _idxList.Clear();
        _cards.Clear();
        _playCards.Clear();

        _cards = GetCardList();
        foreach (var card in _cards)
        {
            await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
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
            await PlayRandom();
        }
        else
        {
            if (_idxList.Length != GameConfiguration.maxCard2Play && Utils.GetPercent(playExtraCardPercent))
            {
                // Play extra cards.
                int extraNum = Random.Range(1, GameConfiguration.maxCard2Play - _idxList.Length + 1);
                var randomIdxList = await GetRandomCardIndex(num: extraNum, hasExceptList: true);
                foreach (var idx in randomIdxList)
                {
                    _idxList.Add(idx);
                }
            }

            foreach (var idx in _idxList)
            {
                await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
                _playCards.Add(_cards[idx]);
            }

        }

        if (_playCards.Count == 0)
        {
            await PlayRandom();
        }
        chosenCardEventSO.RaiseEvent(_playCards);
    }
 
    protected async UniTask<List<int>> GetRandomCardIndex(int num = 1, bool hasExceptList = false)
    {
        if (num > GameConfiguration.maxCard2Play) return null;

        List<int> result = new List<int>();
        await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
        if (!hasExceptList)
        {
            for (int i = 0; i < num; i++)
            {
                await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
                result.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                bool isDuplicated = false;
                await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
                for (int j = 0; j < _idxList.Length; j++)
                {
                    await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
                    if (i == j)
                    {
                        isDuplicated = true;
                        break;
                    }
                }
                if (!isDuplicated) result.Add(i);
                if (result.Count == num) break;
            }
        }

        return result;
    }
}
