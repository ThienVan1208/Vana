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
// public class EndGameEvent
// {
//     public static Action<IPlayable> EndGameAction;
//     public static void RaiseAction(IPlayable player)
//     {
//         EndGameAction?.Invoke(player);
//     }
// }
public class GameManager : MonoBehaviour
{
    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private VoidEventSO _nextTurnEventSO, _continuedCurTurnEventSO;

    // Ref in RuleGameHandler class. 
    [SerializeField] private AddCard2PlayerEventSO _addCard2PlayerEventSO;
    [SerializeField] private PlayableInfoSO _playableInfoSO;
    [SerializeField] private GameConfigSO _gameConfigSO;

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
                    Card newCard = _cardSpawner.GetCards();
                    newCard.gameObject.SetActive(true);
                    playable.AddCards(newCard);
                    await UniTask.Delay(200, cancellationToken: this.GetCancellationTokenOnDestroy());
                }
            }
            _playableInfoSO.GetPlayerByIndex(_playableInfoSO.curPlayerIdx).BeginTurn();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Unitask is cancelled.");
        }

    }
    private void OnEnable()
    {
        _nextTurnEventSO.EventChannel += NextTurn;
        _continuedCurTurnEventSO.EventChannel += ContinueTurn;
        _addCard2PlayerEventSO.EventChannel += AddCards4CurPlayer;
    }
    private void OnDisable()
    {
        _nextTurnEventSO.EventChannel -= NextTurn;
        _continuedCurTurnEventSO.EventChannel -= ContinueTurn;
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
            Debug.Log("Unitask is cancelled.");
        }

    }
}
