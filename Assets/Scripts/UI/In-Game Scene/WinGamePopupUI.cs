using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WinGamePopupUI : PopupUIBase
{
    [SerializeField] private Button _homeButton;
    protected override void Awake()
    {
        _homeButton.onClick.AddListener(() => LoadSceneHandler.LoadSceneByIndex(Constant.HomeScene));
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
