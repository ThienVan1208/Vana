using UnityEngine;

public class Player : PlayerBase, ICardDrawable
{
    // Ref in InGamePanel class.
    [SerializeField] private IntEventSO _drawCardEventSO;

    /* 
    - Ref in InGamePanel class.
    - Used to inform InGamePanel class whether player or opponent is playing cards display
        in order to update or stop update in-game UI. 
    */
    [SerializeField] private BoolEventSO _subcribeCurrencyUIEventSO;


    [Header("Data Events")]
    // Ref in CurrencyManager.
    [SerializeField] private IntEventSO _increaseCurrencyEventSO;

    // Ref in LevelManager.
    [SerializeField] private VoidEventSO _levelUpEventSO;

    [Header("Player UI Prefabs")]
    [SerializeField] private GameObject _handHolderPrefab;
    [SerializeField] private GameObject _buttonPanel;
    private PlayerButtonsPanel _playerButtonPanel;

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
        Vector2 anchorPos = Vector2.zero;

        // Create cardHolder.
        cardHolder = InitPlayerUI(_handHolderPrefab
                                , GameConfiguration.handHolderPos
                                , Quaternion.identity,
                                mainCanvas.gameObject,
                                anchorPos,
                                Vector3.one * GameConfiguration.cardHolderSize).GetComponent<HandHolder>();

        // Create Button Panel.
        _playerButtonPanel = InitPlayerUI(_buttonPanel
                                    , GameConfiguration.playerButtonPanelPos
                                    , Quaternion.identity,
                                    mainCanvas.gameObject,
                                    anchorPos,
                                    Vector3.one * GameConfiguration.cardHolderSize).GetComponent<PlayerButtonsPanel>();

        PopupUIEvent.RaiseAction(PopupUIType.PlayerButtonPanel, active: false);

        _playerButtonPanel.playButtonPrefab.onClick.AddListener(PlayCards);
        _playerButtonPanel.drawCardbuttonPrefab.onClick.AddListener(DrawCard);
        _playerButtonPanel.revealButtonPrefab.onClick.AddListener(RevealCards);
        _playerButtonPanel.passButtonPrefab.onClick.AddListener(PassTurn);

    }
    #endregion

    #region UseCards
    protected override void PlayCards()
    {
        if (!(cardHolder as HandHolder).HelpPlayingCard()) return;

        // DisplayPlayCardUI(false);
        TurnOffPlayUI();
    }
    public override void AddCards(Card card)
    {
        card.SetCardHolder(cardHolder);
        cardHolder.AddCard(card);
    }
    #endregion

    // #region ChangeCards
    // protected override async void ChangeCards()
    // {
    //     base.ChangeCards();
    //     if (!await (cardHolder as HandHolder).HelpChangingCard()) return;
    // }
    // #endregion


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
        _drawCardEventSO.RaiseEvent(_cardDrawNum);
    }
    #endregion


    #region PlayerUI
    private void DisplayPlayCardUI()
    {

        // If it comes to playcard state -> next state is choosing action.
        if (CheckEndGameConditionEvent.RaiseEvent()) return;

        _playerButtonPanel.ActiveButton(_playerButtonPanel.playButtonPrefab, isActive: true);
        _playerButtonPanel.ActiveButton(_playerButtonPanel.drawCardbuttonPrefab, isActive: true);
        _playerButtonPanel.ActiveButton(_playerButtonPanel.passButtonPrefab, isActive: true);
        _playerButtonPanel.ActiveButton(_playerButtonPanel.revealButtonPrefab, isActive: false);

        _playerButtonPanel.ShowPopup();
        GameManagerEvent.RaiseTurnEvent();
        SetCardDrawNum(_cardDrawNum + 1);
        curTurnState = TurnState.ChooseActionState;
    }
    private void DisplayChooseUI()
    {

        // If it comes to choose acion state -> next state is playing cards.
        _playerButtonPanel.ActiveButton(_playerButtonPanel.playButtonPrefab, isActive: false);
        _playerButtonPanel.ActiveButton(_playerButtonPanel.drawCardbuttonPrefab, isActive: false);
        _playerButtonPanel.ActiveButton(_playerButtonPanel.passButtonPrefab, isActive: true);
        _playerButtonPanel.ActiveButton(_playerButtonPanel.revealButtonPrefab, isActive: true);

        _playerButtonPanel.ShowPopup();
        curTurnState = TurnState.PlayCardState;
    }

    private void TurnOnPlayUI(TurnState turnState)
    {
        switch (turnState)
        {
            case TurnState.PlayCardState:
                _subcribeCurrencyUIEventSO.RaiseEvent(true);
                DisplayPlayCardUI();
                break;
            case TurnState.ChooseActionState:
                _subcribeCurrencyUIEventSO.RaiseEvent(false);
                checkRevealEventSO.EventChannel += CheckReveal;
                DisplayChooseUI();
                break;
            default:
                break;
        }
    }
    private void TurnOffPlayUI()
    {
        _playerButtonPanel.HidePopup();
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
            // DisplayChooseUI(false);
            DisplayPlayCardUI();
        }
        else
        {
            TurnOnPlayUI(curTurnState);
        }
    }
    public override void EndTurn()
    {
        base.EndTurn();
        relocatePlayerCardEventSO.EventChannel -= (cardHolder as HandHolder).RelocateCards;
        checkRevealEventSO.EventChannel -= CheckReveal;
    }
    #endregion

    #region RevealPass
    protected override void CheckReveal(bool check)
    {
        base.CheckReveal(check);
        if (check) SuccessRevealCard();
        else FailRevealCard();

    }
    protected override void RevealCards()
    {
        base.RevealCards();
        // DisplayChooseUI(false);
        TurnOffPlayUI();
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
        // DisplayChooseUI(false);
        TurnOffPlayUI();
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
