using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ver0VirtualHandHolder : VirtualHandHolder
{
    protected List<Card> playCards = new List<Card>();
    public override bool HelpPlayingCard()
    {
        GetCardPlayingAI();
        return true;
    }
    protected virtual async void GetCardPlayingAI()
    {
        playCards.Clear();
        await PlayRandom();
        chosenCardEventSO.RaiseEvent(playCards);

    }
    protected async UniTask PlayRandom()
    {
        int ranNum = Mathf.Min(Random.Range(GameConfiguration.minCard2Play, GameConfiguration.maxCard2Play + 1)
                                , curCardNum);

        for (int i = 0; i < ranNum; i++)
        {
            await UniTask.WaitForEndOfFrame(cancellationToken: this.GetCancellationTokenOnDestroy());
            Card newCard = GetCard(disconnect: true);
            if (newCard == null)
            {
                i--;
                Debug.LogWarning("Card " + i + " is null");
                continue;
            }
            playCards.Add(newCard);

        }
    }
}
