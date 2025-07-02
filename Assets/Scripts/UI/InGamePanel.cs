using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class InGamePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _exchangeCardTxt;

    // Ref in HandHolder class.
    [SerializeField] private IntEventSO _exchangeCardEventSO;

    [SerializeField] private TextMeshProUGUI _currencyTxt;

    // Unused yet.
    [SerializeField] private IntEventSO _earnCurrenctEventSO;
    private int _earnedCurrency = 0;
    
    private void OnEnable()
    {
        _exchangeCardEventSO.EventChannel += ChangeCard;
        _earnCurrenctEventSO.EventChannel += EarnCurrency;
    }
    private void OnDisable() {
        _exchangeCardEventSO.EventChannel -= ChangeCard;
        _earnCurrenctEventSO.EventChannel -= EarnCurrency;
    }
    private void Start() {
        EarnCurrency();
    }
    private void ChangeCard(int num)
    {
        _exchangeCardTxt.rectTransform.DOScale(2, 0.2f).SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            _exchangeCardTxt.rectTransform.DOShakeRotation(0.2f, new Vector3(0, 0, 20));
            _exchangeCardTxt.rectTransform.DOScale(1, 0.2f).SetEase(Ease.InOutSine);
        });
        _exchangeCardTxt.text = num.ToString();
    }
    private void EarnCurrency(int num = 0)
    {

        _earnedCurrency += num;
        _currencyTxt.text = "$" + _earnedCurrency.ToString();
    }
}
