using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanelUI : UIBase
{
    [SerializeField] private Button _playButton;
    private void Awake()
    {
        _playButton.onClick.AddListener(() => LoadSceneHandler.LoadSceneByIndex(Constant.PlayScene));
    }
    protected override void Start()
    {
        base.Start();

        // ShowPopup();
    }
    // public override void ShowPopup()
    // {
    //     base.ShowPopup();
    //     popupWindow.SetActive(true);
    // }
    // public override void HidePopup()
    // {
    //     base.HidePopup();
    //     popupWindow.SetActive(false);
    // }
}
