using TMPro;
using UnityEngine;

using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;

public class InGamePanel : UIBase
{
    [Header("Turn UI")]
    [SerializeField] private TextMeshProUGUI _turnTxt;

    // Ref in GameManager class.
    [SerializeField] private IntEventSO _increaseTurnEventSO;

    [SerializeField] private ScaleOutAndShakeEffect _turnUIEffect;



    [Header("Draw Card UI")]
    [SerializeField] private TextMeshProUGUI _cardDrawTxt;

    // Ref in Player class.
    [SerializeField] private IntEventSO _drawCardEventSO;




    [Header("Currency UI")]
    [SerializeField] private TextMeshProUGUI _currencyTxt;
    [SerializeField] private UIEffectBase _currencyContainerEffect;

    // Ref in RuleGameHandler class.
    [SerializeField] private IntEventSO _earnCurrenctEventSO;

    private int _earnedCurrency = 0;
    private ConcurrentQueue<int> _earnedCurrencyQueue = new ConcurrentQueue<int>();
    private bool _earnedCurrencyQueueLock = false;



    protected override void Start()
    {
        base.Start();
        // EarnCurrency();
    }

    private void OnEnable()
    {
        _earnCurrenctEventSO.EventChannel += EarnCurrency;
        _increaseTurnEventSO.EventChannel += IncreaseTurn;
        _drawCardEventSO.EventChannel += DrawCard;
    }
    private void OnDisable()
    {

        _earnCurrenctEventSO.EventChannel -= EarnCurrency;
        _increaseTurnEventSO.EventChannel -= IncreaseTurn;
        _drawCardEventSO.EventChannel -= DrawCard;
    }
    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
    #region Turn
    private void IncreaseTurn(int num)
    {
        _turnTxt.text = num.ToString();
        _turnUIEffect.GetEffect();
    }
    #endregion

    #region Draw card
    private void DrawCard(int num)
    {
        ObjectPoolManager.GetPoolingObject<ExchangeCardEffect>()?.GetEffect(0.5f
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
    private void EarnCurrency(int num = 0)
    {

        // _earnedCurrency += num;
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
        _earnedCurrencyQueueLock = false;
    }
    private async UniTask HelpEarningCurrencyEffect(int num)
    {
        for (int i = 1; i <= num; i++)
        {
            _earnedCurrency += 1;
            _currencyTxt.text = _earnedCurrency.ToString();
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.05f), cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        _currencyContainerEffect.GetEffect();
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.25f), cancellationToken: this.GetCancellationTokenOnDestroy());
    }
    #endregion
}
