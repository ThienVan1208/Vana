using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool FirstTurn = true;

    [SerializeField] private List<Card> _chosenCards = new List<Card>();
    [SerializeField] private ChosenCardEventSO _chosenCardEventSO;
    private List<IPlayable> _playableList = new List<IPlayable>();
    public PlayerBase player, virPlayer;
    private int _curPlayerIdx = 0;
    private void Awake()
    {
        _playableList.Add(player);
        _playableList.Add(virPlayer);
    }
    private void Start()
    {
        // ContinueGame();
    }
    private void OnEnable()
    {
        _chosenCardEventSO.EventChannel += PlayCards;
    }
    private void OnDisable()
    {
        _chosenCardEventSO.EventChannel -= PlayCards;
    }
    private void PlayCards(List<Card> chosenCards)
    {
        _chosenCards = chosenCards;
    }
    public void ContinueGame()
    {
        _playableList[_curPlayerIdx].BeginTurn();
    }

}
