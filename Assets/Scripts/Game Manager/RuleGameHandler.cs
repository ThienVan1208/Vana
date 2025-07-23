using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

public enum PlayerEndGameType
{
    Win,
    Lose
};

// public static class CheckEndGameConditionEvent
// {
//     public static Func<bool> EventChannel;
//     public static bool RaiseEvent()
//     {
//         return EventChannel?.Invoke() ?? false;
//     }
// }


public class RuleGameHandler : MonoBehaviour
{
    public static bool BeginTurn = true;

    [Header("In-Game Events")]
    // Ref in GameManager, PlayerBase classes.
    [SerializeField] private VoidEventSO _nextTurnEventSO;
    [SerializeField] private VoidEventSO _continueTurnEventSO;


    [Header("Game State Events")]
    // Ref in GameManager, PlayerBase classes.
    [SerializeField] private VoidEventSO _endGameEventSO;
    [SerializeField] private VoidEventSO _startGameEventSO;


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

    // Ref in PlayableCardHolder, PlayerBase class.
    [SerializeField] private RetBoolEventSO _checkEndGameEventSO;


    [Header("Related UI Event")]
    // Ref in InGamePanel class.
    [SerializeField] private IntEventSO _earnCurrenctEventSO;

    // Ref in AddCurrencyWhenFlipCardEffect class.
    [SerializeField] private CurrencyFlipCardEffectEventSO _currencyFlipCardEffectEventSO;



    [Header("Audio")]
    // [SerializeField] private AudioClipSO _flipCardAudioClipSO;
    // [SerializeField] private PlayAudioEventSO _playAudioEventO;

    private Vector3 _offset = new Vector3(0, 40, 0);
    private List<Card> _chosenCards = new List<Card>();

    private void OnEnable()
    {
        _revealCardEventSO.EventChannel += RevealCard;
        _passTurnEventSO.EventChannel += PassTurn;
        _chosenCardEventSO.EventChannel += PlayCards;
        _startGameEventSO.EventChannel += StartGame;
        _checkEndGameEventSO.EventChannel += CheckEndGameCond;
        _startGameEventSO.EventChannel += HelpDrawCard;

    }
    private void OnDisable()
    {
        _revealCardEventSO.EventChannel -= RevealCard;
        _passTurnEventSO.EventChannel -= PassTurn;
        _chosenCardEventSO.EventChannel -= PlayCards;
        _startGameEventSO.EventChannel -= StartGame;
        _checkEndGameEventSO.EventChannel -= CheckEndGameCond;
        _startGameEventSO.EventChannel -= HelpDrawCard;
    }
    private void Start()
    {
        // StartGameEvent.RaiseEvent();
        _startGameEventSO.RaiseEvent();
    }

    #region Init
    private async void HelpDrawCard()
    {
        try
        {
            await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
            for (int i = 0; i < GameConfiguration.initCardNum; i++)
            {
                foreach (var playable in _playableInfoSO.GetPlayableList())
                {
                    Card newCard = CardSpawnerEvent.RaiseGetCardEvent();
                    newCard.gameObject.SetActive(true);
                    playable.AddCards(newCard);
                    // _playAudioEventO.RaiseEvent(_flipCardAudioClipSO);
                    await UniTask.Delay(200, cancellationToken: this.GetCancellationTokenOnDestroy());
                }
            }
            _playableInfoSO.GetPlayerByIndex(_playableInfoSO.curPlayerIdx).BeginTurn();
        }
        catch (OperationCanceledException)
        {
            // throw;
        }

    }
    #endregion

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

            // GameManagerEvent.RaiseNextTurnEvent();
            _nextTurnEventSO.RaiseEvent();

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

            await _chosenCards[0].FaceCardUp();

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

                bool revealCondition = _chosenCards[0].GetCardRank() != _chosenCards[i].GetCardRank();

                if (revealCondition)
                {
                    await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
                    await SuccessRevealCard();
                    return;
                }

                _currencyFlipCardEffectEventSO.RaiseEvent(timeDisplay: 1f
                                               , startPos: _chosenCards[i].cardSlotRect.position + _offset
                                               , endPos: _offset
                                               , content: "+" + ((int)_chosenCards[i].GetCardRank()).ToString()
                                               , fontSize: 35
                                               , color: Color.white
                                               , parent: _chosenCards[i].cardSlotRect
                                               , alpha: 1);

                _earnCurrenctEventSO.RaiseEvent((int)_chosenCards[i].GetCardRank());
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

            // GameManagerEvent.RaiseContinueTurnEvent();
            _continueTurnEventSO.RaiseEvent();

            _chosenCards.Clear();
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

            // GameManagerEvent.RaiseNextTurnEvent();
            _nextTurnEventSO.RaiseEvent();

            _chosenCards.Clear();
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

            _nextTurnEventSO.RaiseEvent();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
    #endregion

    #region CheckEndGame
    private bool CheckEndGameCond()
    {
        for (int i = 0; i < _playableInfoSO.GetTotalPlayerNum(); i++)
        {
            var player = _playableInfoSO.GetPlayerByIndex(i);

            // Check player's win condition.
            bool winCondition = player.GetCardNum() == 0;
            if (winCondition)
            {
                DefinePlayerEndGame(i, PlayerEndGameType.Win);
                return true;
            }

            // Check player's lose condition.
            bool LoseCondition = player.GetCardNum() == GameConfiguration.CardCountMaxThreshold
                                || player.GetCardNum() == GameConfiguration.CardCountMinThreshold;
            if (LoseCondition)
            {
                DefinePlayerEndGame(i, PlayerEndGameType.Lose);
                return true;
            }
        }
        return false;
    }
    private async void DefinePlayerEndGame(int playerIndex, PlayerEndGameType type = PlayerEndGameType.Win)
    {
        try
        {
            switch (type)
            {
                case PlayerEndGameType.Win:
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
                    break;

                case PlayerEndGameType.Lose:
                    for (int i = 0; i < _playableInfoSO.GetTotalPlayerNum(); i++)
                    {
                        await UniTask.Delay(200, cancellationToken: this.GetCancellationTokenOnDestroy());
                        if (playerIndex == i)
                        {
                            _playableInfoSO.GetPlayerByIndex(i).LoseGame();
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }


            // EndGameEvent.RaiseEvent();
            _endGameEventSO.RaiseEvent();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
    #endregion

    #region StartGame
    private void StartGame()
    {
        BeginTurn = true;
    }
    #endregion
}
