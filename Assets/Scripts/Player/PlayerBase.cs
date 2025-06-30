using UnityEngine;
public enum TurnState
{
    PlayCardState,
    ChooseActionState
}
public class PlayerBase : MonoBehaviour, IPlayable
{
    [Header("Playable Events")]
    // Ref in RuleGameHandler class.
    [SerializeField] protected VoidEventSO revealCardEventSO;
    [SerializeField] protected VoidEventSO passTurnEventSO;
    [SerializeField] protected VoidEventSO relocatePlayerCardEventSO;
    [SerializeField] protected BoolEventSO checkRevealEventSO;

    [Header("Game Configuration")]
    [SerializeField] protected GameConfigSO gameConfigSO;

    protected Canvas mainCanvas;
    protected CardHolder cardHolder;
    protected TurnState curTurnState;
    protected virtual void Awake()
    {
        // InitPlayableCanvas method must be called before InitCardHolder.
        InitPlayableCanvas();
        InitCardHolder();
    }

    protected virtual void Start()
    {
        curTurnState = TurnState.ChooseActionState;
    }

    protected virtual void InitPlayableCanvas()
    {
        mainCanvas = PlayableCanvasEvent.RaiseGetPlayableCanvasEvent();
    }
    
    #region Interface Declaration
    public virtual void AddCards(Card card) { }
    public virtual void RemoveCards(Card card) { }
    public virtual void BeginTurn()
    {
        Debug.Log(gameObject.name + " turn");    
    }
    public virtual void EndTurn() {}
    public virtual void WinGame(){}
    public virtual void LoseGame(){}

    public virtual int GetCardNum()
    {
        return cardHolder.curCardNum;
    }
    #endregion


    #region Support methods
    protected virtual void PlayCards() { }
    protected virtual void ChangeCards(){ }
    protected virtual void InitCardHolder() { }
    protected virtual void RevealCards() { }
    protected virtual void PassTurn() { }
    protected virtual void CheckReveal(bool check){}
    protected virtual void SuccessRevealCard() { }
    protected virtual void FailRevealCard() {}
    #endregion
}
