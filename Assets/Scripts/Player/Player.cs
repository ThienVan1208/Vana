using System.Globalization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayerBase
{
    [SerializeField] private GameObject _handHolderPrefab;
    [SerializeField] private GameObject _playButtonPrefab;
    [SerializeField] private GameObject _revealButtonPrefab, _passButtonPrefab;

    protected override void InitCardHolder()
    {
        base.InitCardHolder();

        // Create cardHolder.
        Vector2 initCardHolderPos = new Vector2(-146, 85);
        cardHolder = Instantiate(_handHolderPrefab
                        , initCardHolderPos
                        , Quaternion.identity).GetComponent<HandHolder>();

        cardHolder.gameObject.transform.SetParent(mainCanvas.gameObject.transform as RectTransform);

        Vector2 anchorPos = new Vector2(0.5f, 0f);
        (cardHolder.gameObject.transform as RectTransform).anchorMin = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchorMax = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchoredPosition = initCardHolderPos;
        cardHolder.gameObject.transform.localScale = Vector3.one;

        // Create play-card button.
        Vector2 buttonPos = new Vector2(-146f, 15f);

        _playButtonPrefab = Instantiate(_playButtonPrefab, buttonPos, Quaternion.identity);
        _playButtonPrefab.transform.SetParent(mainCanvas.gameObject.transform, false);

        (_playButtonPrefab.transform as RectTransform).anchorMin = anchorPos;
        (_playButtonPrefab.transform as RectTransform).anchorMax = anchorPos;
        (_playButtonPrefab.transform as RectTransform).anchoredPosition = buttonPos;

        _playButtonPrefab.GetComponent<Button>().onClick.AddListener(PlayCards);

        _playButtonPrefab.SetActive(false);
        _playButtonPrefab.transform.localScale = Vector3.one;

        // Create choose action button.
        Vector2 revealButPos = new Vector2(-251f, 15f), passButPos = new Vector2(-36f, 15f);
        _revealButtonPrefab = Instantiate(_revealButtonPrefab, revealButPos, Quaternion.identity);
        _passButtonPrefab = Instantiate(_passButtonPrefab, passButPos, Quaternion.identity);

        _revealButtonPrefab.transform.SetParent(mainCanvas.gameObject.transform, false);
        _passButtonPrefab.transform.SetParent(mainCanvas.gameObject.transform, false);

        (_revealButtonPrefab.transform as RectTransform).anchorMin = anchorPos;
        (_revealButtonPrefab.transform as RectTransform).anchorMax = anchorPos;
        (_revealButtonPrefab.transform as RectTransform).anchoredPosition = revealButPos;

        (_passButtonPrefab.transform as RectTransform).anchorMin = anchorPos;
        (_passButtonPrefab.transform as RectTransform).anchorMax = anchorPos;
        (_passButtonPrefab.transform as RectTransform).anchoredPosition = passButPos;

        _revealButtonPrefab.GetComponent<Button>().onClick.AddListener(RevealCards);
        _passButtonPrefab.GetComponent<Button>().onClick.AddListener(PassTurn);

        _revealButtonPrefab.SetActive(false);
        _passButtonPrefab.SetActive(false);

        _revealButtonPrefab.transform.localScale = Vector3.one;
        _passButtonPrefab.transform.localScale = Vector3.one;
    }
    protected override void Start()
    {
        base.Start();
        _playButtonPrefab.SetActive(false);
    }
    protected override void PlayCards()
    {

        if(!(cardHolder as HandHolder).HelpPlayingCard()) return;
        
        DisplayPlayCardUI(false);
    }
    public override void AddCards(Card card)
    {
        card.SetCardHolder(cardHolder);
        cardHolder.AddCard(card);
        card.CanInteract(true);
    }
    private void DisplayPlayCardUI(bool val = true)
    {
        _playButtonPrefab.SetActive(val);

        // If it comes to playcard state -> next state is choosing action.
        if (val == true) curTurnState = TurnState.ChooseActionState;
    }
    private void DisplayChooseUI(bool val = true)
    {
        _revealButtonPrefab.SetActive(val);
        _passButtonPrefab.SetActive(val);

        // If it comes to choose acion state -> next state is playing cards.
        if (val == true) curTurnState = TurnState.PlayCardState;
    }


    protected override void RevealCards()
    {
        base.RevealCards();
        DisplayChooseUI(false);
        revealCardEventSO.RaiseEvent();
    }
    protected override void PassTurn()
    {
        base.PassTurn();
        DisplayChooseUI(false);
        curTurnState = TurnState.ChooseActionState;
        passTurnEventSO.RaiseEvent();
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        if (RuleGameHandler.BeginTurn)
        {
            RuleGameHandler.BeginTurn = false;
            DisplayChooseUI(false);
            DisplayPlayCardUI();
        }
        else
        {
            // Choose cards to play.
            if (curTurnState == TurnState.PlayCardState)
            {
                DisplayChooseUI(false);
                DisplayPlayCardUI();
            }

            // Choose action.
            else
            {
                checkRevealEventSO.EventChannel += CheckReveal;
                DisplayChooseUI();
                DisplayPlayCardUI(false);
            }

        }
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
}
