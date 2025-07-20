using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

// AI can choose the same rank cards to play.
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
    protected List<Card> cards = new List<Card>();
    protected NativeList<int> cardRankList;
    protected NativeList<int> idxList;
    JobHandle jobHandle;

    protected override void Awake()
    {
        base.Awake();
        cardRankList = new NativeList<int>(allocator: Allocator.Persistent);
        idxList = new NativeList<int>(allocator: Allocator.Persistent);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        cardRankList.Dispose();
        idxList.Dispose();
    }

    protected override async void GetCardPlayingAI()
    {
        cardRankList.Clear();
        idxList.Clear();
        cards.Clear();
        playCards.Clear();

        cards = GetCardList();
        foreach (var card in cards)
        {
            await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
            cardRankList.Add((int)card.GetCardRank());
        }
        FindSameRankCardJob rankJob = new FindSameRankCardJob
        {
            cardRankList = cardRankList,
            idxList = idxList
        };
        jobHandle = rankJob.Schedule();
        jobHandle.Complete();

        if (idxList.Length == 0)
        {
            await PlayRandom();
        }
        else
        {
            if (idxList.Length != GameConfiguration.maxCard2Play && Utils.GetPercent(playExtraCardPercent))
            {
                // Play extra cards.
                int extraNum = Random.Range(1, GameConfiguration.maxCard2Play - idxList.Length + 1);
                var randomIdxList = await GetRandomCardIndex(num: extraNum, hasExceptList: true);
                foreach (var idx in randomIdxList)
                {
                    idxList.Add(idx);
                }
            }

            foreach (var idx in idxList)
            {
                await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
                playCards.Add(cards[idx]);
            }

        }

        if (playCards.Count == 0)
        {
            await PlayRandom();
        }
        chosenCardEventSO.RaiseEvent(playCards);
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
            for (int i = 0; i < cards.Count; i++)
            {
                bool isDuplicated = false;
                await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
                for (int j = 0; j < idxList.Length; j++)
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
