using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private CurrencyInfoSO _currencyInfoSO;
    [SerializeField] private IntEventSO _increaseCurrencyEventSO;
    private void OnEnable()
    {
        _increaseCurrencyEventSO.EventChannel += _currencyInfoSO.IncreaseCurrency;
    }
    private void OnDisable()
    {
        _increaseCurrencyEventSO.EventChannel -= _currencyInfoSO.IncreaseCurrency;
    }
}
