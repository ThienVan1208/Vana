using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
public enum EndGameType
{
    Win,
    Lose,
}

public class GameManager : MonoBehaviour
{
    [Header("In-Game Events")]
    // Ref in RuleGameHandler, PlayerBase classes.
    [SerializeField] private VoidEventSO _nextTurnEventSO;
    [SerializeField] private VoidEventSO _continueTurnEventSO;
    [SerializeField] private VoidEventSO _startTurnEventSO; 

    // Ref in RuleGameHandler class. 
    // When player reveals cards, raise this event to add used cards for current player if revealing fails or for the opponent if revealing successes.
    [SerializeField] private AddCard2PlayerEventSO _addCard2PlayerEventSO;
    [SerializeField] private PlayableInfoSO _playableInfoSO;


    // Ref in InGamePanel.
    [SerializeField] private IntEventSO _displayTurnUIEventSO;
    private int _curTurn = 0;


    [Header("Game State Events")]
    // Ref in RuleGameHandler, PlayerBase classes.
    [SerializeField] private VoidEventSO _endGameEventSO;



    [Header("Playable List")]
    public PlayerBase player;
    public PlayerBase virPlayer;

    public static bool endGame { get; private set; }

    private void Awake()
    {
        endGame = false;

        _playableInfoSO.AddNewPlayer(player);
        _playableInfoSO.AddNewPlayer(virPlayer);

        _playableInfoSO.curPlayerIdx = 0;
        _playableInfoSO.prevPlayerIdx = 0;

    }
    private void OnDestroy()
    {
        _playableInfoSO.ClearPlayableList();
    }

    private void OnEnable()
    {

        _nextTurnEventSO.EventChannel += NextTurn;
        _continueTurnEventSO.EventChannel += ContinueTurn;
        _startTurnEventSO.EventChannel += IncreaseTurn;

        _addCard2PlayerEventSO.EventChannel += AddCards2CurPlayer;

        _endGameEventSO.EventChannel += EndGame;

    }
    private void OnDisable()
    {
        _nextTurnEventSO.EventChannel -= NextTurn;
        _continueTurnEventSO.EventChannel -= ContinueTurn;
        _startTurnEventSO.EventChannel -= IncreaseTurn;

        _addCard2PlayerEventSO.EventChannel -= AddCards2CurPlayer;

        _endGameEventSO.EventChannel -= EndGame;

    }

    

    #region Turn API
    private void NextTurn()
    {
        _playableInfoSO.prevPlayerIdx = _playableInfoSO.curPlayerIdx;
        _playableInfoSO.curPlayerIdx = (_playableInfoSO.curPlayerIdx + 1) % _playableInfoSO.GetTotalPlayerNum();

        _playableInfoSO.GetPlayerByIndex(_playableInfoSO.prevPlayerIdx).EndTurn();
        _playableInfoSO.GetPlayerByIndex(_playableInfoSO.curPlayerIdx).BeginTurn();
    }
    private void ContinueTurn()
    {
        _playableInfoSO.GetPlayerByIndex(_playableInfoSO.curPlayerIdx).BeginTurn();
    }

    private void IncreaseTurn()
    {
        _curTurn += 1;
        _displayTurnUIEventSO.RaiseEvent(_curTurn);
    }
    #endregion

    #region Add card API
    private async void AddCards2CurPlayer(int playerIndex, List<Card> cards)
    {
        await HelpAddCards2CurPlayer(playerIndex, cards, 0.2f);
    }
    private async UniTask HelpAddCards2CurPlayer(int playerIndex, List<Card> cards, float time)
    {
        try
        {
            foreach (var card in cards)
            {
                await UniTask.Delay((int)(time * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
                _playableInfoSO.GetPlayerByIndex(playerIndex).AddCards(card);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }

    }
    #endregion

    #region EndGame
    private void EndGame()
    {
        endGame = true;
    }
    #endregion
}
