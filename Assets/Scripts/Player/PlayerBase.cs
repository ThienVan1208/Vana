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

    [SerializeField] protected Canvas mainCanvas;
    [SerializeField] protected CardHolder cardHolder;
    protected TurnState curTurnState;
    protected virtual void Awake()
    {
        // InitPlayableCanvas method must be called before InitCardHolder.
        InitPlayableCanvas();
        InitCardHolder();
    }
    // protected virtual void OnValidate()0
    // {
    //     InitPlayableCanvas();
    //     InitCardHolder();
    // }
    protected virtual void OnEnable()
    {
        EndGameEvent.EventChannel += EndGame;
    }
    protected virtual void OnDisable()
    {
        EndGameEvent.EventChannel -= EndGame;
    }
    protected virtual void Start()
    {
        curTurnState = TurnState.ChooseActionState;
    }
    protected virtual void OnDestroy()
    {
        cardHolder = null;
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
    public virtual void EndTurn() { }
    public virtual void WinGame() { }
    public virtual void LoseGame() { }

    public virtual int GetCardNum()
    {
        return cardHolder.curCardNum;
    }
    #endregion


    #region Support methods
    protected virtual void PlayCards() { }
    protected virtual void ChangeCards() { }
    protected virtual void InitCardHolder() { }
    protected virtual void RevealCards() { }
    protected virtual void PassTurn() { }
    protected virtual void CheckReveal(bool check) { }
    protected virtual void SuccessRevealCard() { }
    protected virtual void FailRevealCard() { }
    protected virtual GameObject InitPlayerUI(GameObject prefab
                                    , Vector2 pos
                                    , Quaternion quarternion
                                    , GameObject parent
                                    , Vector2 anchorPos
                                    , Vector2 localScale)
    {
        Debug.Log("init");
        GameObject playerUI = Instantiate(prefab, pos, quarternion);
        playerUI.transform.SetParent(parent.transform, false);

        RectTransform playerUIRect = playerUI.transform as RectTransform;
        playerUIRect.anchorMin = anchorPos;
        playerUIRect.anchorMax = anchorPos;
        playerUIRect.anchoredPosition = pos;

        playerUI.transform.localScale = localScale;
        return playerUI;
    }

    protected virtual void EndGame() { }
    #endregion
}
