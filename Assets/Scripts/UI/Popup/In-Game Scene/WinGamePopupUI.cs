using System;
using UnityEngine;
using UnityEngine.UI;

public class WinGamePopupUI : PopupUIBase
{
    [SerializeField] private Button _homeButton;
    private UIEffectBase _uiEffect;
    protected override void Awake()
    {
        _homeButton.onClick.AddListener(() => LoadSceneHandler.LoadSceneByIndex(Constant.HomeScene));
        _uiEffect = popupWindow.GetComponent<UIEffectBase>();
    }
    public override void ShowPopup(Action callback = null)
    {
        base.ShowPopup(callback);
        popupWindow.SetActive(true);
        _uiEffect.GetEffect(callback: ()=> callback?.Invoke());
    }
    public override void HidePopup(Action callback = null)
    {
        base.HidePopup(callback);
        popupWindow.SetActive(false);
        callback?.Invoke();
    }

}
