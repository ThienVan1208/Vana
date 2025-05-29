using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VirtualPlayer : PlayerBase
{
    [SerializeField] private GameObject _handHolderPrefab;
    
    // Used to assign chosen card list in RuleGameHandle.
    [SerializeField] protected ChosenCardEventSO chosenCardEventSO;

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
        card.FaceCardDown();
        cardHolder.AddCard(card);
        card.CanInteract(false);
    }
    protected override void RevealCards()
    {
        base.RevealCards();
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
        await UniTask.Delay(2000);
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
                int ranAction = Random.Range(0, 2);
                if (ranAction == 0)
                {
                    PassTurn();
                }
                else
                {
                    RevealCards();
                }
                curTurnState = TurnState.PlayCardState;
            }
            else
            {
                PlayCards();
                curTurnState = TurnState.ChooseActionState;
            }
        }
        await UniTask.WaitForEndOfFrame();
    }
    protected override void PlayCards()
    {
        List<Card> cards = new List<Card>();
        int ranNum = Random.Range(2, 5);
        for (int i = 0; i < ranNum; i++)
        {
            cards.Add(cardHolder.GetCard(disconnect: true));
        }
        chosenCardEventSO.RaiseEvent(cards);
    }
}
