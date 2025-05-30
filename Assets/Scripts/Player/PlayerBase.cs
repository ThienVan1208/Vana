using UnityEngine;
public enum TurnState
{
    PlayCardState,
    ChooseActionState
}
public class PlayerBase : MonoBehaviour, IPlayable
{
    // Ref in RuleGameHandler class.
    [SerializeField] protected VoidEventSO revealCardEventSO, passTurnEventSO;

    [SerializeField] protected BoolEventSO checkRevealEventSO;
    [SerializeField] protected Canvas mainCanvas;
    protected CardHolder cardHolder;
    protected TurnState curTurnState;
    protected virtual void Awake()
    {
        InitCardHolder();
    }
    protected virtual void Start()
    {
        curTurnState = TurnState.ChooseActionState;
    }

    // IPlayable Declaration.
    public virtual void AddCards(Card card) { }
    public virtual void RemoveCards(Card card) { }
    public virtual void BeginTurn()
    {
        Debug.Log(gameObject.name + " turn");    
    }
    public virtual void EndTurn() { }


    protected virtual void PlayCards() { }
    protected virtual void InitCardHolder() { }
    protected virtual void RevealCards() { }
    protected virtual void PassTurn() { }
    protected virtual void CheckReveal(bool check){}
    protected virtual void SuccessRevealCard() { }
    protected virtual void FailRevealCard() {}
}
