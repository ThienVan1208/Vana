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

    }

    #region Init
    protected override void InitCardHolder()
    {
        base.InitCardHolder();
        Vector2 anchorPos = Vector2.zero;

        // Create cardHolder.
        cardHolder = InitPlayerUI(_handHolderPrefab
                                , gameConfigSO.handHolderPos
                                , Quaternion.identity,
                                mainCanvas.gameObject,
                                anchorPos,
                                Vector3.one * gameConfigSO.cardHolderSize).GetComponent<HandHolder>();

        // Create play-card button.
        _playButtonPrefab = InitPlayerUI(_playButtonPrefab
                                    , gameConfigSO.inGameLeftButtonPos
                                    , Quaternion.identity
                                    , mainCanvas.gameObject
                                    , anchorPos
                                    , Vector2.one);
        _playButtonPrefab.SetActive(false);
        _playButtonPrefab.GetComponent<Button>().onClick.AddListener(PlayCards);

        // Create change-card button.
        _changeButtonPrefab = InitPlayerUI(_changeButtonPrefab
                                    , gameConfigSO.inGameRightButtonPos
                                    , Quaternion.identity
                                    , mainCanvas.gameObject
                                    , anchorPos
                                    , Vector2.one);
        _changeButtonPrefab.SetActive(false);
        _changeButtonPrefab.GetComponent<Button>().onClick.AddListener(ChangeCards);

        // Create choose-action button.
        _revealButtonPrefab = InitPlayerUI(_revealButtonPrefab
                                    , gameConfigSO.inGameLeftButtonPos
                                    , Quaternion.identity
                                    , mainCanvas.gameObject
                                    , anchorPos
                                    , Vector2.one);
        _revealButtonPrefab.SetActive(false);
        _revealButtonPrefab.GetComponent<Button>().onClick.AddListener(RevealCards);

        _passButtonPrefab = InitPlayerUI(_passButtonPrefab
                                    , gameConfigSO.inGameRightButtonPos
                                    , Quaternion.identity
                                    , mainCanvas.gameObject
                                    , anchorPos
                                    , Vector2.one);
        _passButtonPrefab.SetActive(false);
        _passButtonPrefab.GetComponent<Button>().onClick.AddListener(PassTurn);

    }
    #endregion

    #region UseCards
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

    #region ChangeCards
    protected override async void ChangeCards()
    {
        base.ChangeCards();
        if (!await (cardHolder as HandHolder).HelpChangingCard()) return;
    }
    #endregion

    #region PlayerUI
    private void DisplayPlayCardUI(bool val = true)
    {
        _playButtonPrefab.SetActive(val);
        _changeButtonPrefab.SetActive(val);

        // If it comes to playcard state -> next state is choosing action.
        if (val == true)
        {
            curTurnState = TurnState.ChooseActionState;
            (cardHolder as HandHolder).AddChangeCardNum();
        }
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

    #region Reveal&Pass
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

    #region Endgame
    // These Win/LoseGame methods are used by RuleGameHandler to get reward/punish.
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

    // Used to set end-game condition.
    protected override void EndGame()
    {
        base.EndGame();
        EndTurn();
    }
    #endregion
}
