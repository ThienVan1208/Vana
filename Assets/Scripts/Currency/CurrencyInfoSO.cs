using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyInfoSO", menuName = "CurrencyInfoSO")]
public class CurrencyInfoSO : ScriptableObject
{
    [SerializeField] private int _currency;
    public void IncreaseCurrency(int val)
    {
        _currency += val;
    }
    public int GetCurrency()
    {
        return _currency;
    }
}

