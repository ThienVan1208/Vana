using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VirtualPlayer : PlayerBase, ICardDrawable
{
    [Header("Player UI Prefab")]
    [SerializeField] private GameObject _handHolderPrefab;

    private int _cardDrawNum;

    protected override void Start()
    {
        base.Start();
        SetCardDrawNum(3);
    }

    #region Init
    protected override void InitCardHolder()
    {
        base.InitCardHolder();

        // Create cardHolder.
        Vector2 anchorPos = new Vector2(0f, 1f);
        cardHolder = InitPlayerUI(_handHolderPrefab
                                , GameConfiguration.virtualHolderPos
                                , Quaternion.identity,
                                mainCanvas.gameObject,
                                anchorPos,
                                Vector3.one * GameConfiguration.cardHolderSize).GetComponent<VirtualHandHolder>();
    }
    #endregion

    #region Play card
    public override void AddCards(Card card)
    {

        card.SetCardHolder(cardHolder);
        _ = card.FaceCardDown();
        cardHolder.AddCard(card);
        card.CanInteract(false);
    }
    protected override void CheckReveal(bool check)
    {
        base.CheckReveal(check);
        if (check) SuccessRevealCard();
        else FailRevealCard();

    }
    protected override void SuccessRevealCard()
    {
        base.SuccessRevealCard();
        curTurnState = TurnState.PlayCardState;
    }
    protected override void FailRevealCard()
    {
        base.FailRevealCard();
        curTurnState = TurnState.ChooseActionState;
    }
    protected override void PlayCards()
    {
        SetCardDrawNum(_cardDrawNum + 1);
        
        (cardHolder as VirtualHandHolder).HelpPlayingCard();

    }
    protected override void RevealCards()
    {
        base.RevealCards();
        checkRevealEventSO.EventChannel += CheckReveal;
        revealCardEventSO.RaiseEvent();
    }
    protected override void PassTurn()
    {
        base.PassTurn();
        curTurnState = TurnState.ChooseActionState;
        passTurnEventSO.RaiseEvent();
    }
    #endregion

    #region Draw card
    public void DrawCard()
    {
        if (_cardDrawNum <= 0)
        {
            Debug.Log("No more draw card.");
            return;
        }
        AddCards(CardSpawnerEvent.RaiseGetCardEvent(isActive: true));
        SetCardDrawNum(_cardDrawNum - 1);
    }

    public int GetCardDrawNum()
    {
        return _cardDrawNum;
    }

    public void SetCardDrawNum(int num = 1)
    {
        if (num > GameConfiguration.maxCardDrawNum || num < 0) return;
        _cardDrawNum = num;
        // _drawCardEventSO.RaiseEvent(_cardDrawNum);
    }
    #endregion

    #region In turn
    public override void BeginTurn()
    {
        if (GameManager.endGame) return;

        _ = HelpBeginTurn();
    }
    private async UniTask HelpBeginTurn()
    {
        relocatePlayerCardEventSO.EventChannel += (cardHolder as VirtualHandHolder).RelocateCards;
        await UniTask.Delay(1000);
        if (RuleGameHandler.BeginTurn)
        {
            if (CheckEndGameConditionEvent.RaiseEvent()) return;
            GameManagerEvent.RaiseTurnEvent();
            

            RuleGameHandler.BeginTurn = false;
            base.BeginTurn();
            PlayCards();
            curTurnState = TurnState.ChooseActionState;

        }
        else
        {
            if (curTurnState == TurnState.ChooseActionState)
            {
                curTurnState = TurnState.PlayCardState;
                int ranAction = Random.Range(0, 3);
                if (ranAction == 0)
                {
                    PassTurn();
                }
                else
                {
                    RevealCards();
                }

            }
            else
            {
                if (CheckEndGameConditionEvent.RaiseEvent()) return;
                GameManagerEvent.RaiseTurnEvent();
                
                PlayCards();
                curTurnState = TurnState.ChooseActionState;
            }
        }
        await UniTask.WaitForEndOfFrame();
    }

    public override void EndTurn()
    {
        base.EndTurn();
        relocatePlayerCardEventSO.EventChannel -= (cardHolder as VirtualHandHolder).RelocateCards;
        checkRevealEventSO.EventChannel -= CheckReveal;
    }
    #endregion


    #region End game
    protected override void EndGame()
    {
        base.EndGame();
        EndTurn();
    }

    
    #endregion
}
