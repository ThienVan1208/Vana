using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using UnityEditor;
public class RuleGameHandler : MonoBehaviour
{
    public static bool BeginTurn = true;

    [Header("Playable Informations")]
    [SerializeField] private PlayableInfoSO _playableInfoSO;

    [Header("Card Holder Events")]
    // Ref in TableHolder class.
    [SerializeField] private IntEventSO _activeCardSlotEventSO;
    [SerializeField] private CardEventSO _moveCardToTableEventSO;
    [SerializeField] private VoidEventSO _refeshTableEventSO;

    [Header("Playable Events")]
    // Used in class PlayerBase.
    [SerializeField] private VoidEventSO _revealCardEventSO;
    [SerializeField] private VoidEventSO _passTurnEventSO;

    /*
    - If player failed / successed in revealing -> raises this event with argument false / true. 
    - Ref in playerBase class.
    */
    [SerializeField] private BoolEventSO _checkRevealEventSO;

    /* 
    - Used to receive chosen card list from player.
    - Ref in PlayerBase class.
    */
    [SerializeField] private ChosenCardEventSO _chosenCardEventSO;

    /*
    - Ref in GameManager class. 
    - Used to add used card list to playeable.
    - If the current playable reveals successfully -> add cards to the previous playable.
        else adding cards to current playable.
    */
    [SerializeField] private AddCard2PlayerEventSO _addCard2PlayerEventSO;

    /* 
    - Ref in playerBase class.
    - After playing cards, raises this event to relocate all cards in hand.
    */
    [SerializeField] private VoidEventSO _relocatePlayerCardEventSO;


    private List<Card> _chosenCards = new List<Card>();

    private void OnEnable()
    {
        _revealCardEventSO.EventChannel += RevealCard;
        _passTurnEventSO.EventChannel += PassTurn;

        _chosenCardEventSO.EventChannel += PlayCards;
    }
    private void OnDisable()
    {
        _revealCardEventSO.EventChannel -= RevealCard;
        _passTurnEventSO.EventChannel -= PassTurn;

        _chosenCardEventSO.EventChannel -= PlayCards;
    }

    #region Play card
    /*
    - This func means when playable one plays cards and ends turn
        , after that it comes to the opponent's turn (choosing reveal cards or pass). 
    */
    private void PlayCards(List<Card> chosenCards)
    {
        _chosenCards = new List<Card>(chosenCards);
        _ = HelpPlayCards();
    }
    private async UniTask HelpPlayCards()
    {
        try
        {
            // Get flip cards effects.
            await GetFlipCardWhenPlay();

            _relocatePlayerCardEventSO.RaiseEvent();

            if (!CheckEndGameCond())
            {
                GameManagerEvent.RaiseNextTurnEvent();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }

    // Used to move choosen card list to table and face them down excluding the first card.
    private async UniTask GetFlipCardWhenPlay()
    {
        try
        {
            _activeCardSlotEventSO.RaiseEvent(_chosenCards.Count);

            _ = _chosenCards[0].FaceCardUp();

            _moveCardToTableEventSO.RaiseEvent(_chosenCards[0]);

            for (int i = 1; i < _chosenCards.Count; i++)
            {
                await UniTask.Delay(300, cancellationToken: this.GetCancellationTokenOnDestroy());
                _ = _chosenCards[i].FaceCardDown();

                _moveCardToTableEventSO.RaiseEvent(_chosenCards[i]);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }

    }
    #endregion

    #region Reveal card
    private void RevealCard()
    {
        _ = HelpRevealingCard();
    }
    private async UniTask HelpRevealingCard()
    {
        try
        {
            for (int i = 1; i < _chosenCards.Count; i++)
            {
                await _chosenCards[i].FaceCardUp(hasTransition: true);
                await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());

                bool revealCondition = _chosenCards[0].GetCardSuit() != _chosenCards[i].GetCardSuit()
                            && _chosenCards[0].GetCardRank() != _chosenCards[i].GetCardRank();

                if (revealCondition)
                {
                    await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
                    await SuccessRevealCard();
                    return;
                }
            }

            await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
            await FailRevealCard();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
    private async UniTask SuccessRevealCard()
    {
        try
        {
            // Add choosen card list to usedCardQueue.
            await UsedCardContainerEvent.RaiseAddUsedCardEvent(_chosenCards);

            _addCard2PlayerEventSO.RaiseEvent(_playableInfoSO.prevPlayerIdx
                                    , UsedCardContainerEvent.RaiseGetUsedCardListEvent());

            await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());

            _refeshTableEventSO.RaiseEvent();

            _checkRevealEventSO.RaiseEvent(true);

            GameManagerEvent.RaiseContinueTUrnEvent();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
    private async UniTask FailRevealCard()
    {
        try
        {
            // Add choosen card list to usedCardQueue.
            await UsedCardContainerEvent.RaiseAddUsedCardEvent(_chosenCards);

            _addCard2PlayerEventSO.RaiseEvent(_playableInfoSO.curPlayerIdx
                                    , UsedCardContainerEvent.RaiseGetUsedCardListEvent());

            await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());

            // _tableHolder.RefreshTable();
            _refeshTableEventSO.RaiseEvent();

            BeginTurn = true;
            _checkRevealEventSO.RaiseEvent(false);

            GameManagerEvent.RaiseNextTurnEvent();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
    #endregion

    #region Pass turn
    private void PassTurn()
    {
        Debug.Log("pass turn");

        _ = HelpPassTurn();
    }
    private async UniTask HelpPassTurn()
    {
        try
        {
            // Add choosen card list to usedCardQueue.
            await UsedCardContainerEvent.RaiseAddUsedCardEvent(_chosenCards);

            BeginTurn = true;

            _refeshTableEventSO.RaiseEvent();

            GameManagerEvent.RaiseNextTurnEvent();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
    #endregion

    #region EndGame
    private bool CheckEndGameCond()
    {
        for (int i = 0; i < _playableInfoSO.GetTotalPlayerNum(); i++)
        {
            if (_playableInfoSO.GetPlayerByIndex(i).GetCardNum() == 0)
            {
                _ = EndGame(i);
                return true;
            }
        }
        return false;
    }
    private async UniTask EndGame(int playerIndex)
    {
        try
        {
            for (int i = 0; i < _playableInfoSO.GetTotalPlayerNum(); i++)
            {
                await UniTask.Delay(200, cancellationToken: this.GetCancellationTokenOnDestroy());
                if (playerIndex == i)
                {
                    _playableInfoSO.GetPlayerByIndex(i).WinGame();
                }
                else
                {
                    _playableInfoSO.GetPlayerByIndex(i).LoseGame();
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
    #endregion
}
