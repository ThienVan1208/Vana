using UnityEngine;

public class InfoPanelUI : PopupUIBase
{
    protected override void Start()
    {
        base.Start();

        ShowPopup();
    }
    public override void ShowPopup()
    {
        base.ShowPopup();
        popupWindow.SetActive(true);
    }
    public override void HidePopup()
    {
        base.HidePopup();
        popupWindow.SetActive(false);
    }
}
