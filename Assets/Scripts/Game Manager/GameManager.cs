using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
public enum EndGameType
{
    Win,
    Lose,
}
public static class GameManagerEvent
{
    public static Action NextTurnEvent;
    public static void RaiseNextTurnEvent()
    {
        NextTurnEvent?.Invoke();
    }

    public static Action ContinueTurnEvent;
    public static void RaiseContinueTUrnEvent()
    {
        ContinueTurnEvent?.Invoke();
    }
}
public class GameManager : MonoBehaviour
{
    [Header("In-Game Events")]
    // Ref in RuleGameHandler class. 
    [SerializeField] private AddCard2PlayerEventSO _addCard2PlayerEventSO;
    [SerializeField] private PlayableInfoSO _playableInfoSO;

    [Header("Game Configuration")]
    [SerializeField] private GameConfigSO _gameConfigSO;

    [Header("Playable List")]
    public PlayerBase player, virPlayer;

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
    private void Start()
    {
        _ = HelpDrawCard();
    }

    private async UniTask HelpDrawCard()
    {
        try
        {
            await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
            for (int i = 0; i < _gameConfigSO.initCardNum; i++)
            {
                foreach (var playable in _playableInfoSO.GetPlayableList())
                {
                    Card newCard = CardSpawnerEvent.RaiseGetCardEvent();
                    newCard.gameObject.SetActive(true);
                    playable.AddCards(newCard);
                    await UniTask.Delay(200, cancellationToken: this.GetCancellationTokenOnDestroy());
                }
            }
            _playableInfoSO.GetPlayerByIndex(_playableInfoSO.curPlayerIdx).BeginTurn();
        }
        catch (OperationCanceledException)
        {
            throw;
        }

    }
    private void OnEnable()
    {
        GameManagerEvent.NextTurnEvent += NextTurn;
        GameManagerEvent.ContinueTurnEvent += ContinueTurn;
        
        _addCard2PlayerEventSO.EventChannel += AddCards4CurPlayer;
    }
    private void OnDisable()
    {
        GameManagerEvent.NextTurnEvent -= NextTurn;
        GameManagerEvent.ContinueTurnEvent -= ContinueTurn;

        _addCard2PlayerEventSO.EventChannel -= AddCards4CurPlayer;
    }
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

    private void AddCards4CurPlayer(int playerIndex, List<Card> cards)
    {
        _ = HelpAddCards4CurPlayer(playerIndex, cards, 0.2f);
    }
    private async UniTask HelpAddCards4CurPlayer(int playerIndex, List<Card> cards, float time)
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
}
