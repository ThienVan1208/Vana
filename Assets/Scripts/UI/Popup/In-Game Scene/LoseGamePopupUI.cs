using UnityEngine;
using UnityEngine.UI;

public class LoseGamePopupUI : PopupUIBase
{
    [SerializeField] private Button _homeButton;
    private UIEffectBase _uiEffect;
    protected override void Awake()
    {
        _homeButton.onClick.AddListener(() => LoadSceneHandler.LoadSceneByIndex(Constant.HomeScene));
        _uiEffect = popupWindow.GetComponent<UIEffectBase>();
    }
    public override void ShowPopup()
    {
        base.ShowPopup();
        popupWindow.SetActive(true);
        _uiEffect.GetEffect();
    }
    public override void HidePopup()
    {
        base.HidePopup();
        popupWindow.SetActive(false);
    }
}
