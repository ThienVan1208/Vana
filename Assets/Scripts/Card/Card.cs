using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler
                    , IPointerClickHandler, IDragHandler, IPointerEnterHandler
{

    [HideInInspector]
    public RectTransform cardSlotRect, myRect;



    [Header("Card Info")]
    [SerializeField] private CardInfoSO _cardInfoSO;
    public RectTransform frontImg, backImg;

    

    [Header("Audio")]
    public AudioClipSO choseCardAudioClipSO;
    public AudioClipSO flipCardAudioClipSO;
    public PlayAudioEventSO playAudioEventSO;


    public FSM stateMachine { get; private set; }
    private IdleState _idleState;
    private DragState _dragState;
    private HoverState _hoverState;
    private MoveToTargetState _moveState;
    private ClickState _clickState;
    private FlipState _flipState;
    private bool _canInteract = true;



    [Header("Input")]
    public InteractInputReaderSO interactInputReaderSO;





    public CardHolder cardHolder { get; private set; }
    public bool isDestroy { get; private set; } = false;

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
        try
        {
            isDestroy = true;
            stateMachine.StopAllState();

            _idleState.DestroyState();
            _dragState.DestroyState();
            _moveState.DestroyState();
            _hoverState.DestroyState();
            _clickState.DestroyState();
            _flipState.DestroyState();


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
        catch (Exception)
        {
            Debug.LogWarning("Card OnDestroy was cancelled.");
        }

    }

// #if UNITY_EDITOR
//     private void OnValidate()
//     {
//         bool changed = false;

//         // Assign ChoseCardAudioClipSO
//         if (choseCardAudioClipSO == null)
//         {
//             choseCardAudioClipSO = FindAssetByName<AudioClipSO>("ChoseCardAudioClipSO");
//             if (choseCardAudioClipSO != null)
//             {
//                 Debug.Log($"Automatically assigned AudioClipSO: {choseCardAudioClipSO.name}", this);
//                 changed = true;
//             }
//         }

//         // Assign PlayOneShotAudioEventSO
//         if (playAudioEventSO == null)
//         {
//             playAudioEventSO = FindAssetByName<PlayAudioEventSO>("PlayOneShotAudioEventSO");
//             if (playAudioEventSO != null)
//             {
//                 Debug.Log($"Automatically assigned PlayAudioEventSO: {playAudioEventSO.name}", this);
//                 changed = true;
//             }
//         }

//         // Assign FlipCardAudioClipSO
//         if (flipCardAudioClipSO == null)
//         {
//             flipCardAudioClipSO = FindAssetByName<AudioClipSO>("FlipCardAudioClipSO");
//             if (flipCardAudioClipSO != null)
//             {
//                 Debug.Log($"Automatically assigned AudioClipSO: {flipCardAudioClipSO.name}", this);
//                 changed = true;
//             }
//             else
//             {
//                 Debug.LogWarning("Could not find FlipCardAudioClipSO. Please ensure it exists in the project.");
//             }
//         }

//         // If we made any changes, mark the component as 'dirty' so Unity saves them.
//         if (changed)
//         {
//             EditorUtility.SetDirty(this);
//         }
//     }

//     /// <summary>
//     /// Finds the first ScriptableObject asset of a given type and name in the project.
//     /// </summary>
//     /// <typeparam name="T">The type of ScriptableObject to find.</typeparam>
//     /// <param name="assetName">The exact filename (without extension) of the asset.</param>
//     /// <returns>The found asset, or null if it doesn't exist.</returns>
//     private T FindAssetByName<T>(string assetName) where T : UnityEngine.Object
//     {
//         // Construct the search filter for AssetDatabase
//         // "t:TypeName assetName" finds assets of TypeName with the exact name.
//         string searchFilter = $"t:{typeof(T).Name} {assetName}";
//         string[] guids = AssetDatabase.FindAssets(searchFilter);

//         if (guids.Length == 0)
//         {
//             Debug.LogWarning($"Could not find an asset of type '{typeof(T).Name}' with the name '{assetName}'.");
//             return null;
//         }

//         if (guids.Length > 1)
//         {
//             Debug.LogWarning($"Found multiple assets named '{assetName}'. Loading the first one. Please ensure asset names are unique if this is not intended.");
//         }
        
//         string path = AssetDatabase.GUIDToAssetPath(guids[0]);
//         return AssetDatabase.LoadAssetAtPath<T>(path);
//     }
// #endif




    /*
        In unity, when the canvas render mode is world, so the recttransform of all UI elements in that canvas is equal to transform?
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
    public void GetMove(RectTransform target, bool getAudio = true)
    {
        if(getAudio)
        {
            playAudioEventSO.RaiseEvent(flipCardAudioClipSO);
        }
    

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
            throw;
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
    public void GetIdleEffect(bool val = true)
    {
        _idleState.didIdle = val;

    }
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
    Ace = 1,
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