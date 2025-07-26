using TMPro;
using UnityEngine;

using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System;

public class InGamePanel : UIBase
{
    [Header("Turn")]
    [SerializeField] private TextMeshProUGUI _turnTxt;

    // Ref in GameManager class.
    [SerializeField] private IntEventSO _increaseTurnEventSO;

    [SerializeField] private ScaleOutAndShakeEffect _turnUIEffect;



    [Header("Draw Card")]
    [SerializeField] private TextMeshProUGUI _cardDrawTxt;

    /* 
    - Ref in Player class.
    - Used to display drawCard num UI of player.
    */
    [SerializeField] private IntEventSO _drawCardEventSO;

    /* 
    - Ref in DrawCardEffect class.
    - Used to get effect when displaying drawCard num UI.
    */
    [SerializeField] private DrawCardEffectEventSO _drawCardEffectEventSO;




    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI _currencyTxt;
    [SerializeField] private UIEffectBase _currencyContainerEffect;

    // Ref in RuleGameHandler class.
    [SerializeField] private IntEventSO _earnCurrenctEventSO;

    /* 
    - Ref in Player class.
    - Player uses this event to inform InGamePanel class 
        whether player or opponent is playing cards
        in order to update or stop update in-game UI. 
    */
    [SerializeField] private BoolEventSO _subcribeCurrencyUIEventSO;

    /* 
    - Used to check whether _subcribeCurrencyUIEventSO is called 
        with the same argument continuously. 
    - Ex: can not:
            SubscribeCurrencyUI(true); 
            SubscribeCurrencyUI(true);
        or:
            SubscribeCurrencyUI(false); 
            SubscribeCurrencyUI(false);
    */
    private bool _checkSubcribeDuplicate = false;

    private int _earnedCurrency = 0;
    private ConcurrentQueue<int> _earnedCurrencyQueue = new ConcurrentQueue<int>();
    private bool _earnedCurrencyQueueLock = false;




    [Header("Current Card Num")]
    [SerializeField] private TextMeshProUGUI _playerCurrentCardNumTxt;
    [SerializeField] private TextMeshProUGUI _opponentCurrentCardNumTxt;

    [SerializeField] private TextMeshProUGUI _playerMaxCardNumTxt;
    [SerializeField] private TextMeshProUGUI _opponentMaxCardNumTxt;

    // Ref in PlayableCardHolder class.
    [SerializeField] private IntEventSO _playerCurrentCardNumEventSO;
    [SerializeField] private IntEventSO _opponentCurrentCardNumEventSO;





    [Header("Audio")]
    [SerializeField] private AudioClipSO _currencyAudioClipSO;
    [SerializeField] private PlayAudioEventSO _playAudioEventSO;

    protected override void Start()
    {
        base.Start();
        InitMaxCardNum();
    }

    protected override void OnEnable()
    {
        _increaseTurnEventSO.EventChannel += IncreaseTurn;
        _drawCardEventSO.EventChannel += DrawCard;
        _subcribeCurrencyUIEventSO.EventChannel += SubscribeCurrencyUI;
        _playerCurrentCardNumEventSO.EventChannel += UpdatePlayerCurrentCardNum;
        _opponentCurrentCardNumEventSO.EventChannel += UpdateOpponentCurrentCardNum;

        SubscribeCurrencyUI(true);
    }
    protected override void OnDisable()
    {
        _increaseTurnEventSO.EventChannel -= IncreaseTurn;
        _drawCardEventSO.EventChannel -= DrawCard;
        _subcribeCurrencyUIEventSO.EventChannel -= SubscribeCurrencyUI;
        _playerCurrentCardNumEventSO.EventChannel -= UpdatePlayerCurrentCardNum;
        _opponentCurrentCardNumEventSO.EventChannel -= UpdateOpponentCurrentCardNum;

        SubscribeCurrencyUI(false);
    }
    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
    #region Turn
    private void IncreaseTurn(int num)
    {
        _turnTxt.text = num.ToString();
        _turnUIEffect.GetEffect(callback: null);
    }
    #endregion

    #region Draw card
    private void DrawCard(int num)
    {
        _drawCardEffectEventSO.RaiseEvent(0.5f
                                        , _cardDrawTxt.rectTransform.position
                                        , new Vector3(80f, 80f, 0f)
                                        , _cardDrawTxt.text
                                        , 40
                                        , Color.red
                                        , 0.7f);

        _cardDrawTxt.rectTransform.DOScale(2, 0.2f).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            _cardDrawTxt.rectTransform.DOShakeRotation(0.2f, new Vector3(0, 0, 20));
            _cardDrawTxt.rectTransform.DOScale(1, 0.2f).SetEase(Ease.InOutSine);
        });
        _cardDrawTxt.text = num.ToString();
    }
    #endregion

    #region Currency
    // Subcribe event used to update currency UI.
    private void SubscribeCurrencyUI(bool val)
    {
        if (val == _checkSubcribeDuplicate) return;

        _checkSubcribeDuplicate = val;
        if (val) _earnCurrenctEventSO.EventChannel += EarnCurrency;
        else _earnCurrenctEventSO.EventChannel -= EarnCurrency;
    }
    private void EarnCurrency(int num = 0)
    {
        _earnedCurrencyQueue.Enqueue(num);

        if (!_earnedCurrencyQueueLock) GetEarnCurrencyEffect();
    }
    private async void GetEarnCurrencyEffect()
    {
        _earnedCurrencyQueueLock = true;
        while (_earnedCurrencyQueue.TryDequeue(out var num))
        {
            await HelpEarningCurrencyEffect(num);
        }

        _currencyContainerEffect.GetEffect(callback: null);
        _playAudioEventSO.RaiseEvent(_currencyAudioClipSO);
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.25f), cancellationToken: this.GetCancellationTokenOnDestroy());

        _earnedCurrencyQueueLock = false;
    }
    private async UniTask HelpEarningCurrencyEffect(int num)
    {
        try
        {
            for (int i = 1; i <= num; i++)
            {
                _earnedCurrency += 1;
                _currencyTxt.text = _earnedCurrency.ToString();
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.05f), cancellationToken: this.GetCancellationTokenOnDestroy());
            }

        }
        catch (OperationCanceledException)
        {

        }

    }
    #endregion

    #region Current Card Num
    private void InitMaxCardNum()
    {
        _playerMaxCardNumTxt.text = "/" + (GameConfiguration.CardCountMaxThreshold - 1).ToString();
        _opponentMaxCardNumTxt.text = "/" + (GameConfiguration.CardCountMaxThreshold - 1).ToString();
    }
    private void UpdatePlayerCurrentCardNum(int num)
    {
        Debug.Log("UpdateOpponentCurrentCardNum: " + num);
        _playerCurrentCardNumTxt.rectTransform.DOScale(2, 0.2f).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            _playerCurrentCardNumTxt.rectTransform.DOShakeRotation(0.2f, new Vector3(0, 0, 20));
            _playerCurrentCardNumTxt.rectTransform.DOScale(1, 0.2f).SetEase(Ease.InOutSine);
        });
        _playerCurrentCardNumTxt.text = num.ToString();
    }
    private void UpdateOpponentCurrentCardNum(int num)
    {
        Debug.Log("UpdateOpponentCurrentCardNum: " + num);
        _opponentCurrentCardNumTxt.rectTransform.DOScale(2, 0.2f).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            _opponentCurrentCardNumTxt.rectTransform.DOShakeRotation(0.2f, new Vector3(0, 0, 20));
            _opponentCurrentCardNumTxt.rectTransform.DOScale(1, 0.2f).SetEase(Ease.InOutSine);
        });
        _opponentCurrentCardNumTxt.text = num.ToString();
    }
    #endregion
}
