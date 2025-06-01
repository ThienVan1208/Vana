using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CardSpawner _cardSpawner;
    private List<IPlayable> _playableList = new List<IPlayable>();
    public PlayerBase player, virPlayer;
    [SerializeField] private VoidEventSO _nextTurnEventSO, _continuedCurTurnEventSO;

    // Ref in RuleGameHandler class. 
    [SerializeField] private AddCard2PlayerEventSO _addCard2PlayerEventSO;
    [SerializeField] private PlayableInfoSO _playableInfoSO;
    private void Awake()
    {
        _playableList.Add(player);
        _playableList.Add(virPlayer);
        
        _playableInfoSO.curPlayerIdx = 0;
        _playableInfoSO.prevPlayerIdx = 0;
    }
    private void Start()
    {
        // ContinueGame();
        StartCoroutine(Wait2DrawCard());
    }
    private IEnumerator Wait2DrawCard()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 10; i++)
        {
            foreach (var p in _playableList)
            {
                Card newCard = _cardSpawner.GetCards();
                newCard.gameObject.SetActive(true);
                p.AddCards(newCard);
                yield return new WaitForSeconds(0.2f);
            }

        }
        _playableList[_playableInfoSO.curPlayerIdx].BeginTurn();
    }
    private async UniTask HelpDrawCard()
    {
        await UniTask.Delay(1000);
        for (int i = 0; i < 10; i++)
        {
            foreach (var p in _playableList)
            {
                Card newCard = _cardSpawner.GetCards();
                newCard.gameObject.SetActive(true);
                p.AddCards(newCard);
                await UniTask.Delay(200);
            }

        }
        _playableList[_playableInfoSO.curPlayerIdx].BeginTurn();
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
        _playableInfoSO.curPlayerIdx = (_playableInfoSO.curPlayerIdx + 1) % _playableList.Count;

        _playableList[_playableInfoSO.prevPlayerIdx].EndTurn();
        _playableList[_playableInfoSO.curPlayerIdx].BeginTurn();
    }
    private void ContinueTurn()
    {
        _playableList[_playableInfoSO.curPlayerIdx].BeginTurn();
    }


    private void AddCards4CurPlayer(int playerIndex, List<Card> cards)
    {
        _ = HelpAddCards4CurPlayer(playerIndex, cards, 0.2f);
    }
    private async UniTask HelpAddCards4CurPlayer(int playerIndex, List<Card> cards, float time)
    {
        foreach (var card in cards)
        {
            await UniTask.Delay((int)(time * 1000));
            _playableList[playerIndex].AddCards(card);
            // card.CanInteract(true);
        }
    }



}
