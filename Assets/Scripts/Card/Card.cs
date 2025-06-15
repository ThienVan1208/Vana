using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler
                    , IPointerClickHandler, IDragHandler, IPointerEnterHandler
{
    [SerializeField] private CardInfoSO _cardInfoSO;
    [SerializeField] public GameConfigSO gameConfigSO;

    [HideInInspector]
    public RectTransform cardSlotRect, myRect;

    public RectTransform frontImg, backImg;
    public FSM stateMachine { get; private set; }
    private IdleState _idleState;
    private DragState _dragState;
    private HoverState _hoverState;
    private MoveToTargetState _moveState;
    private ClickState _clickState;
    private FlipState _flipState;
    private bool _canInteract = true;
    public CardHolder cardHolder { get; private set; }

    // 2 * _time2HaflRotate is the total time for card to rotate (used to flip card).
    private float _time2HaflRotate = 0.3f;
    private void Awake()
    {

        cardSlotRect = transform.parent as RectTransform;
        myRect = transform as RectTransform;


        // Init StateBase & StateMachine.
        stateMachine = new FSM();
        _idleState = new IdleState(this);
        _dragState = new DragState(this);
        _moveState = new MoveToTargetState(this);
        _hoverState = new HoverState(this);
        _clickState = new ClickState(this);
        _flipState = new FlipState(this);

        stateMachine.SetDefaultState(_idleState);
        stateMachine.InitFSM();

        // Add transition for states.
        stateMachine.AddTransit(_dragState, _idleState);
        stateMachine.AddTransit(_moveState, _idleState);
        stateMachine.AddTransit(_hoverState, _idleState);
        stateMachine.AddTransit(_clickState, _idleState);
        stateMachine.AddTransit(_flipState, _idleState);
    }
    private void OnDestroy()
    {
        // stateMachine.StopAllState();

        stateMachine = null;
        _idleState = null;
        _dragState = null;
        _moveState = null;
        _hoverState = null;
        _clickState = null;
        _flipState = null;
        cardSlotRect = null;
        myRect = null;
    }

    // private void Start() {
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

    #region Card slot
    // cardSlot is the parent of card.
    public void SetCardSlot(RectTransform cardSlot)
    {
        cardSlotRect = cardSlot;
    }
    public void SetCardHolder(CardHolder holder)
    {
        cardHolder = holder;
        if (!_canInteract) return;
    }
    public void SetCardParent(RectTransform parent)
    {
        if (parent == null) return;
        myRect.SetParent(parent, false);
    }
    #endregion

    private void Update()
    {
        stateMachine.Update();
    }

    #region Move state
    // Set @_cardSlot to target and then move it to target.
    public void GetMove(RectTransform target)
    {

        // Check card is up.
        if (!_clickState.IsClick())
        {
            // Disconnect with handHolder api.
            _clickState.SetChosenFlag(false);

            // Get down.
            stateMachine.ChangeState(_clickState);

            // Connect again.
            _clickState.SetChosenFlag(true);
        }

        SetCardSlot(target);
        stateMachine.ChangeState(_moveState, isForce: true);
    }
    #endregion

    #region Drag state
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canInteract) return;

        stateMachine.ChangeState(_dragState);
    }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData)
    {
        _dragState.EndDrag();
    }
    #endregion

    #region Click state
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_canInteract) return;
        stateMachine.ChangeState(_clickState);
    }
    #endregion

    #region Hover state
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canInteract) return;
        stateMachine.ChangeState(_hoverState);
    }
    #endregion

    #region Flip card
    public async UniTask FaceCardDown(bool hasTransition = false)
    {
        try
        {
            if (hasTransition)
            {
                stateMachine.StopAllState();
                Vector3 rotateDir = new Vector3(0f, 90f, 0f);
                myRect.transform.localEulerAngles = Vector3.zero;
                myRect.DORotate(myRect.transform.localEulerAngles + rotateDir, _time2HaflRotate).SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    frontImg.gameObject.SetActive(false);
                    backImg.gameObject.SetActive(true);
                    myRect.DORotate(myRect.transform.localEulerAngles + rotateDir, _time2HaflRotate).SetEase(Ease.InOutCubic)
                    .OnComplete(() => stateMachine.ContinuePrevState());
                });
                await UniTask.Delay((int)(2 * _time2HaflRotate), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            else
            {
                frontImg.gameObject.SetActive(false);
                backImg.gameObject.SetActive(true);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Unitask is cancelled.");
        }
    }

    public async UniTask FaceCardUp(bool hasTransition = false)
    {
        try
        {
            if (hasTransition)
            {
                stateMachine.StopAllState();
                Vector3 rotateDir = new Vector3(0f, 90f, 0f);
                myRect.transform.localEulerAngles = Vector3.zero;
                myRect.DORotate(myRect.transform.localEulerAngles + rotateDir, _time2HaflRotate).SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    // await UniTask.Delay(500);
                    backImg.gameObject.SetActive(false);
                    frontImg.gameObject.SetActive(true);

                    // await UniTask.Delay(500);

                    myRect.DORotate(myRect.transform.localEulerAngles - rotateDir, _time2HaflRotate).SetEase(Ease.InOutCubic)
                    .OnComplete(() =>
                    {
                        stateMachine.ContinuePrevState();
                        stateMachine.ChangeState(_flipState);
                    });
                });
                await UniTask.Delay((int)(2 * _time2HaflRotate), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            else
            {
                frontImg.gameObject.SetActive(true);
                backImg.gameObject.SetActive(false);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Unitask is cancelled.");
        }

    }
    #endregion

    #region Utils
    public CardSuit GetCardSuit()
    {
        return _cardInfoSO.cardSuit;
    }
    public CardRank GetCardRank()
    {
        return _cardInfoSO.cardRank;
    }
    public void CanInteract(bool val = true)
    {
        _canInteract = val;
    }
    public bool IsInteractable() { return _canInteract; }
    #endregion
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