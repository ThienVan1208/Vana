using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System.Threading.Tasks;
public class RuleGameHandler : MonoBehaviour
{
    public static bool BeginTurn = true;
    [SerializeField] private TableHolder _tableHolder;

    // Used in class PlayerBase.
    [SerializeField] private VoidEventSO _revealCardEventSO, _passTurnEventSO;
    [SerializeField] private VoidEventSO _nextTurnEventSO, _continuedCurTurnEventSO;

    // Used to receive chosen card list from playable one in PlayerBase.
    [SerializeField] private ChosenCardEventSO _chosenCardEventSO;
    [SerializeField] private UsedCardHolder _usedCardHolder;
    [SerializeField] private PlayableInfoSO _playableInfoSO;

    /*
    - Is referenced in GameManager class. 
    - Used to add used card list to playeable.
    - If the current playable reveals successfully -> add cards to the previous playable.
        else adding cards to current playable.
    */
    [SerializeField] private AddCard2PlayerEventSO _addCard2PlayerEventSO;
    [SerializeField] private CamShakeEventSO _campShakeEventSO;
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
        // Get flip cards effects.
        await GetFlipCardWhenPlay();

        _nextTurnEventSO.RaiseEvent();
    }
    // Used to move choosen card list to table and face them down excluding the first card.
    private async UniTask GetFlipCardWhenPlay()
    {
        _ = _chosenCards[0].FaceCardUp();
        _tableHolder.AddCard(_chosenCards[0]);
        for (int i = 1; i < _chosenCards.Count; i++)
        {
            await UniTask.Delay(300);
            _chosenCards[i].FaceCardDown();
            _tableHolder.AddCard(_chosenCards[i]);
        }
    }

    private void RevealCard()
    {
        _ = HelpRevealingCard();
    }
    private async UniTask HelpRevealingCard()
    {
        for (int i = 1; i < _chosenCards.Count; i++)
        {
            await _chosenCards[i].FaceCardUp(hasTransition: true);
            _campShakeEventSO.RaiseEvent(0.25f, 1f);
            await UniTask.Delay(1000);
            if (_chosenCards[0].GetCardRank() != _chosenCards[i].GetCardRank())
            {
                _ = FailRevealCard();
                return;
            }
        }
        _ = SuccessRevealCard();
    }
    private async UniTask SuccessRevealCard()
    {
        DisconnectCardsFromTable(_chosenCards);

        // Add choosen card list to usedCardQueue.
        _usedCardHolder.AddUsedCards(_chosenCards);

        _addCard2PlayerEventSO.RaiseEvent(_playableInfoSO.prevPlayerIdx
                                , _usedCardHolder.GetUsedCardList());

        await UniTask.Delay(1000);
        _continuedCurTurnEventSO.RaiseEvent();
    }
    private async UniTask FailRevealCard()
    {
        DisconnectCardsFromTable(_chosenCards);

        // Add choosen card list to usedCardQueue.
        _usedCardHolder.AddUsedCards(_chosenCards);


        _addCard2PlayerEventSO.RaiseEvent(_playableInfoSO.curPlayerIdx
                                , _usedCardHolder.GetUsedCardList());

        await UniTask.Delay(1000);
        _nextTurnEventSO.RaiseEvent();
    }
    private void DisconnectCardsFromTable(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            _tableHolder.DisconnectCardSlot(card);
        }
    }
    private void PassTurn()
    {
        DisconnectCardsFromTable(_chosenCards);
        _ = HelpPassTurn();
    }
    private async UniTask HelpPassTurn()
    {
        // Add choosen card list to usedCardQueue.
        _usedCardHolder.AddUsedCards(_chosenCards);
        BeginTurn = true;

        await UniTask.Delay(1000);
        _nextTurnEventSO.RaiseEvent();
    }
    private void Update() {
        if (BeginTurn == true)
        {
            Debug.Log("begin turn is true");
        }
    }
    
}
