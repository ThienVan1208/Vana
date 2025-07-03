using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VirtualPlayer : PlayerBase
{
    [Header("Player UI Prefab")]
    [SerializeField] private GameObject _handHolderPrefab;
    protected override void InitCardHolder()
    {
        base.InitCardHolder();

        // Create cardHolder.
        Vector2 anchorPos = new Vector2(0f, 1f);
        cardHolder = InitPlayerUI(_handHolderPrefab
                                , gameConfigSO.virtualHolderPos
                                , Quaternion.identity,
                                mainCanvas.gameObject,
                                anchorPos,
                                Vector3.one * gameConfigSO.cardHolderSize).GetComponent<VirtualHandHolder>();
    }
    public override void AddCards(Card card)
    {

        card.SetCardHolder(cardHolder);
        _ = card.FaceCardDown();
        cardHolder.AddCard(card);
        card.CanInteract(false);
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
                if (ranAction == 10)
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
        (cardHolder as VirtualHandHolder).HelpPlayingCard();
        // if ((cardHolder as VirtualHandHolder).GetCardNum() == 0) EndGameEvent.RaiseAction(this);
    }

    protected override void EndGame()
    {
        base.EndGame();
        EndTurn();
    }
}
