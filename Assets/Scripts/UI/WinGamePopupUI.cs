using UnityEngine;

public class WinGamePopupUI : PopupUIBase
{
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
