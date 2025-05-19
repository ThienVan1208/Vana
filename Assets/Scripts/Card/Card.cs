using Mono.Cecil.Cil;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
};
public enum CardRank
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler
                    , IPointerClickHandler, IDragHandler, IPointerEnterHandler
{
    [SerializeField] private CardInfoSO _cardInfoSO;

    [HideInInspector]
    public RectTransform cardSlotRect, myRect;

    public RectTransform frontImg, backImg;
    private FSM _stateMachine;
    private IdleState _idleState;
    private DragState _dragState;
    private HoverState _hoverState;
    private MoveToTargetState _moveState;
    private ClickState _clickState;
    private bool _canInteract = true;
    public CardHolder cardHolder { get; private set; }
    private void Awake()
    {
        cardSlotRect = transform.parent as RectTransform;
        myRect = transform as RectTransform;

        // Init StateBase & StateMachine.
        _stateMachine = new FSM();
        _idleState = new IdleState(_stateMachine, this);
        _dragState = new DragState(_stateMachine, this);
        _moveState = new MoveToTargetState(_stateMachine, this);
        _hoverState = new HoverState(_stateMachine, this);
        _clickState = new ClickState(_stateMachine, this);

        _stateMachine.SetDefaultState(_idleState);
        _stateMachine.InitFSM();

        // Add transition for states.
        _stateMachine.AddTransit(_dragState, _idleState);
        _stateMachine.AddTransit(_moveState, _idleState);
        _stateMachine.AddTransit(_hoverState, _idleState);
        _stateMachine.AddTransit(_clickState, _idleState);
    }
    // private void OnValidate()
    // {
    //     RectTransform shadowRect = gameObject.transform.Find("Shadow") as RectTransform;
    //     shadowRect.GetComponent<Image>().raycastTarget = false;

    // }

    // cardSlot is the parent of card.
    public void SetCardSlot(RectTransform cardSlot)
    {
        cardSlotRect = cardSlot;
    }
    public void SetCardHolder(CardHolder holder)
    {
        cardHolder = holder;
        if (!_canInteract) return;
        _dragState.SetCardHolder(cardHolder);
        _hoverState.SetCardHolder(cardHolder);
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    // Set @_cardSlot to target and then move it to target.
    public void GetMove(RectTransform target)
    {
        SetCardSlot(target);
        _stateMachine.ChangeState(_moveState, isForce: true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canInteract) return;

        _stateMachine.ChangeState(_dragState, isForce: true);
    }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData)
    {
        _dragState.EndDrag();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_canInteract) return;
        _stateMachine.ChangeState(_clickState);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canInteract) return;
        _stateMachine.ChangeState(_hoverState);
    }

    public void FaceCardDown()
    {
        frontImg.gameObject.SetActive(false);
        backImg.gameObject.SetActive(true);
    }
    public void FaceCardUp()
    {
        frontImg.gameObject.SetActive(true);
        backImg.gameObject.SetActive(false);
    }
    public void CanInteract(bool val)
    {
        _canInteract = val;
    }
    public bool IsInteractable(){ return _canInteract; }
}
