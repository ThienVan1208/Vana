using UnityEngine;
using UnityEngine.UI;

public class PlayerButtonsPanel : PopupUIBase
{
    public Button playButtonPrefab;
    public Button drawCardbuttonPrefab;
    public Button revealButtonPrefab;
    public Button passButtonPrefab;
    [SerializeField] private BottomTopPopupEffect _uiEffect;
    public override void ShowPopup()
    {
        base.ShowPopup();
        SetEffectInfo(initYPos: -50, finalYPos: 0);
        popupWindow.SetActive(true);
        _uiEffect.GetEffect(callback: () => popupWindow.SetActive(true));
        
    }
    public override void HidePopup()
    {
        base.HidePopup();
        SetEffectInfo(initYPos: 0, finalYPos: -50);
        _uiEffect.GetEffect(callback: () => popupWindow.SetActive(false));
    }
    public void ActiveButton(Button button, bool isActive = true)
    {
        button.gameObject.transform.parent.gameObject.SetActive(isActive);
    }
    public void SetEffectInfo(float duration = 0.25f, float initYPos = -20, float finalYPos = 0)
    {
        _uiEffect.duration = duration;
        _uiEffect.startYPos = initYPos;
        _uiEffect.endYPos = finalYPos;
    }
}
