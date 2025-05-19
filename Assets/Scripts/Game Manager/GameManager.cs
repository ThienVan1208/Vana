using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool FirstTurn = true;

    [SerializeField] private List<Card> _chosenCards = new List<Card>();
    [SerializeField] private ChosenCardEventSO _chosenCardEventSO;
    [SerializeField] private CardSpawner _cardSpawner;
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
        StartCoroutine(Wait2DrawCard());
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
        _chosenCards = new List<Card>(chosenCards);
    }
    public void ContinueGame()
    {
        _playableList[_curPlayerIdx].BeginTurn();
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
    }

}
