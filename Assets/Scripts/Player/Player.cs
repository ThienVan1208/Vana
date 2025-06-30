using System.Globalization;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayerBase
{
    [Header("Data Events")]
    // Ref in CurrencyManager.
    [SerializeField] private IntEventSO _increaseCurrencyEventSO;

    // Ref in LevelManager.
    [SerializeField] private VoidEventSO _levelUpEventSO;

    [Header("Player UI Prefabs")]
    [SerializeField] private GameObject _handHolderPrefab;
    [SerializeField] private GameObject _playButtonPrefab;
    [SerializeField] private GameObject _changeButtonPrefab;
    [SerializeField] private GameObject _revealButtonPrefab;
    [SerializeField] private GameObject _passButtonPrefab;

    protected override void Start()
    {
        base.Start();
        _playButtonPrefab.SetActive(false);
    }

    #region Init
    protected override void InitCardHolder()
    {
        base.InitCardHolder();

        // Create cardHolder.
        Vector3 initCardHolderPos = new Vector3(283, 85, 0f);
        cardHolder = Instantiate(_handHolderPrefab
                        , initCardHolderPos
                        , Quaternion.identity).GetComponent<HandHolder>();

        cardHolder.gameObject.transform.SetParent(mainCanvas.gameObject.transform as RectTransform, false);

        Vector2 anchorPos = new Vector2(0f, 0f);
        (cardHolder.gameObject.transform as RectTransform).anchorMin = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchorMax = anchorPos;
        (cardHolder.gameObject.transform as RectTransform).anchoredPosition = initCardHolderPos;
        cardHolder.gameObject.transform.localScale = Vector3.one * gameConfigSO.cardHolderSize;


        // Create play-card button.
        Vector2 buttonPos = new Vector2(181f, 15f);

        _playButtonPrefab = Instantiate(_playButtonPrefab, buttonPos, Quaternion.identity);
        _playButtonPrefab.transform.SetParent(mainCanvas.gameObject.transform, false);

        (_playButtonPrefab.transform as RectTransform).anchorMin = anchorPos;
        (_playButtonPrefab.transform as RectTransform).anchorMax = anchorPos;
        (_playButtonPrefab.transform as RectTransform).anchoredPosition = buttonPos;

        _playButtonPrefab.GetComponent<Button>().onClick.AddListener(PlayCards);

        _playButtonPrefab.SetActive(false);
        _playButtonPrefab.transform.localScale = Vector3.one;

        // Create change-card button.
        Vector2 changeButtonPos = new Vector2(396f, 15f);
        _changeButtonPrefab = Instantiate(_changeButtonPrefab, changeButtonPos, Quaternion.identity);
        _changeButtonPrefab.transform.SetParent(mainCanvas.gameObject.transform, false);

        (_changeButtonPrefab.transform as RectTransform).anchorMin = anchorPos;
        (_changeButtonPrefab.transform as RectTransform).anchorMax = anchorPos;
        (_changeButtonPrefab.transform as RectTransform).anchoredPosition = changeButtonPos;

        _changeButtonPrefab.GetComponent<Button>().onClick.AddListener(ChangeCards);

        _changeButtonPrefab.SetActive(false);
        _changeButtonPrefab.transform.localScale = Vector3.one;

        // Create choose-action button.
        Vector2 revealButPos = new Vector2(181f, 15f), passButPos = new Vector2(396f, 15f);
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
    #endregion

    #region Use cards
    protected override void PlayCards()
    {
        if (!(cardHolder as HandHolder).HelpPlayingCard()) return;

        DisplayPlayCardUI(false);
    }
    public override void AddCards(Card card)
    {
        card.SetCardHolder(cardHolder);
        cardHolder.AddCard(card);
        card.CanInteract(true);
    }
    #endregion

    #region Change cards
    protected override async void ChangeCards()
    {
        base.ChangeCards();
        if (! await (cardHolder as HandHolder).HelpChangingCard()) return;
    }
    #endregion

    #region Player UI
    private void DisplayPlayCardUI(bool val = true)
    {
        _playButtonPrefab.SetActive(val);
        _changeButtonPrefab.SetActive(val);

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
    #endregion

    #region In turn
    public override void BeginTurn()
    {
        base.BeginTurn();
        relocatePlayerCardEventSO.EventChannel += (cardHolder as HandHolder).RelocateCards;
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
        relocatePlayerCardEventSO.EventChannel -= (cardHolder as HandHolder).RelocateCards;
        checkRevealEventSO.EventChannel -= CheckReveal;
    }
    #endregion

    #region Reveal/Pass
    protected override void CheckReveal(bool check)
    {
        base.CheckReveal(check);
        if (check) SuccessRevealCard();
        else FailRevealCard();

    }
    protected override void RevealCards()
    {
        base.RevealCards();
        DisplayChooseUI(false);
        revealCardEventSO.RaiseEvent();
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

    protected override void PassTurn()
    {
        base.PassTurn();
        DisplayChooseUI(false);
        curTurnState = TurnState.ChooseActionState;
        passTurnEventSO.RaiseEvent();
    }
    #endregion

    #region End game
    public override void WinGame()
    {
        base.WinGame();
        PopupUIEvent.RaiseAction(PopupUIType.WinGame);
        _increaseCurrencyEventSO.RaiseEvent(5);
        _levelUpEventSO.RaiseEvent();
        SaveDataEvent.RaiseAction();
    }
    public override void LoseGame()
    {
        base.LoseGame();
        PopupUIEvent.RaiseAction(PopupUIType.LoseGame);
        _increaseCurrencyEventSO.RaiseEvent(-5);
        SaveDataEvent.RaiseAction();
    }
    #endregion
}
