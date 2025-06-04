using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler
                    , IPointerClickHandler, IDragHandler, IPointerEnterHandler
{
    [SerializeField] private CardInfoSO _cardInfoSO;
    [SerializeField] public GameConfigSO gameConfigSO;

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

    // 2 * _time2HaflRotate is the total time for card to rotate (used to flip card).
    private float _time2HaflRotate = 0.5f;
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
    // private void Start() 
    //     myRect.localScale = Vector3.one * _gameConfigSO.cardSize;
    // }
    //     private void OnValidate()
    //     {
    //         // Only run if _gameConfigSO is not already assigned
    //         if (gameConfigSO == null)
    //         {
    //             // Find the ScriptableObject in the asset database
    //             string[] guids = AssetDatabase.FindAssets($"t:{typeof(GameConfigSO).Name}");
    //             if (guids.Length > 0)
    //             {
    //                 string path = AssetDatabase.GUIDToAssetPath(guids[0]);
    //                 gameConfigSO = AssetDatabase.LoadAssetAtPath<GameConfigSO>(path);
    //                 if (gameConfigSO != null)
    //                 {
    //                     Debug.Log($"Assigned ScriptableObject: {gameConfigSO.name}");
    // #if UNITY_EDITOR
    //                     // Mark the object as dirty to ensure the change is saved
    //                     EditorUtility.SetDirty(this);
    // #endif
    //                 }
    //                 else
    //                 {
    //                     Debug.LogWarning($"No GameConfigSO found in the project.");
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogWarning($"No GameConfigSO found in the project.");
    //             }
    //         }
    //     }
    /*
    in unity, when the canvas render mode is world, so the recttransform of all UI elements in that canvas is equal to transform?
    */

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
    public void SetParent(RectTransform parent)
    {
        myRect.SetParent(parent, false);
    }
    public CardSuit GetCardSuit()
    {
        return _cardInfoSO.cardSuit;
    }
    public CardRank GetCardRank()
    {
        return _cardInfoSO.cardRank;
    }
    private void Update()
    {
        _stateMachine.Update();
    }

    // Set @_cardSlot to target and then move it to target.
    public void GetMove(RectTransform target)
    {

        // Check card is up.
        if (!_clickState.IsClick())
        {
            _clickState.SetChosenFlag(false);

            // Get down.
            _stateMachine.ChangeState(_clickState);

            _clickState.SetChosenFlag(true);
        }

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

    public async UniTask FaceCardDown(bool hasTransition = false)
    {

        if (hasTransition)
        {
            _stateMachine.StopAllState();
            Vector3 rotateDir = new Vector3(0f, 90f, 0f);
            myRect.transform.localEulerAngles = Vector3.zero;
            myRect.DORotate(myRect.transform.localEulerAngles + rotateDir, _time2HaflRotate)
            .OnComplete(() =>
            {
                frontImg.gameObject.SetActive(false);
                backImg.gameObject.SetActive(true);
                myRect.DORotate(myRect.transform.localEulerAngles + rotateDir, _time2HaflRotate)
                .OnComplete(() => _stateMachine.ContinuePrevState());
            });
            await UniTask.Delay((int)(2 * _time2HaflRotate));
        }
        else
        {
            frontImg.gameObject.SetActive(false);
            backImg.gameObject.SetActive(true);
        }
    }

    public async UniTask FaceCardUp(bool hasTransition = false)
    {

        if (hasTransition)
        {
            _stateMachine.StopAllState();
            Vector3 rotateDir = new Vector3(0f, 90f, 0f);
            myRect.transform.localEulerAngles = Vector3.zero;
            myRect.DORotate(myRect.transform.localEulerAngles + rotateDir, _time2HaflRotate)
            .OnComplete(() =>
            {
                // await UniTask.Delay(500);
                backImg.gameObject.SetActive(false);
                frontImg.gameObject.SetActive(true);

                // await UniTask.Delay(500);

                myRect.DORotate(myRect.transform.localEulerAngles - rotateDir, _time2HaflRotate)
                .OnComplete(() =>
                {
                    _stateMachine.ContinuePrevState();
                    _stateMachine.ChangeState(_hoverState, isForce: true);
                });
            });
            await UniTask.Delay((int)(2 * _time2HaflRotate));
        }
        else
        {
            frontImg.gameObject.SetActive(true);
            backImg.gameObject.SetActive(false);
        }
    }
    public void CanInteract(bool val = true)
    {
        _canInteract = val;
    }
    public bool IsInteractable() { return _canInteract; }
}
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