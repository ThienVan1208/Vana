using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VirtualPlayer : PlayerBase
{
    [SerializeField] private GameObject _handHolderPrefab;
    private Vector2 _initCardHolderPos = new Vector2(-146f, -50f);
    protected override void InitCardHolder()
    {
        base.InitCardHolder();

        // Create cardHolder.
        cardHolder = Instantiate(_handHolderPrefab
                        , _initCardHolderPos
                        , Quaternion.identity).GetComponent<VirtualHandHolder>();

        cardHolder.gameObject.transform.SetParent(mainCanvas.gameObject.transform as RectTransform);

        Vector2 anchorPos = new Vector2(0.5f, 1f);
        (cardHolder.gameObject.transform as RectTransform).anchorMin = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchorMax = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchoredPosition = _initCardHolderPos;

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
        _ = HelpBeginTurn();
    }
    private async UniTask HelpBeginTurn()
    {
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
                PlayCards();
                curTurnState = TurnState.ChooseActionState;
            }
        }
        await UniTask.WaitForEndOfFrame();
    }

    public override void EndTurn()
    {
        base.EndTurn();
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
    }
}
