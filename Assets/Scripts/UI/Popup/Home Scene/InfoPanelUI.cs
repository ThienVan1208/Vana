using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUI : PopupUIBase
{
    [SerializeField] private LevelInfoSO _levelInfoSO;
    [SerializeField] private CurrencyInfoSO _currencyInfoSO;
    [SerializeField] private TextMeshProUGUI _currencyTxt, _levelTxt;
    protected override void Start()
    {
        base.Start();

        ShowPopup();
    }
    public override void ShowPopup()
    {
        base.ShowPopup();
        popupWindow.SetActive(true);
        UpdateCurrency();
        UpdateLevel();
    }
    public override void HidePopup()
    {
        base.HidePopup();
        popupWindow.SetActive(false);
    }
    private void UpdateCurrency()
    {
        _currencyTxt.text = "$ " + _currencyInfoSO.GetCurrency().ToString();
    }
    private void UpdateLevel()
    {
        _levelTxt.text = _levelInfoSO.GetLevel().ToString();
    }
}
