using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public enum PopupUIType
{
    WinGame,
    LoseGame,
    PlayPanel,
    InforPanel,
    Setting,
}
public static class PopupUIEvent
{
    public static Action<PopupUIType, bool> displayPopupAction;
    public static void RaiseAction(PopupUIType type, bool active = true)
    {
        displayPopupAction?.Invoke(type, active);
    } 
}
public class UIPopupManager : MonoBehaviour
{
    /*
    - Ref in PopupUIBase class.
    - Used to store popup UI which is present in the current scene.
    */
    [SerializeField] private PopupUIEventSO _subcribedPopupUIEventSO;
    private Dictionary<PopupUIType, PopupUIBase> _popupUIDics = new Dictionary<PopupUIType, PopupUIBase>();

    // Used to handle popup request -> avoid race condition.
    private ConcurrentQueue<PopupUIType> _popupRequest = new ConcurrentQueue<PopupUIType>();
    private bool _lockPopupHandler = false;

    // Used to count the number of popup UI which is active.
    private int _countPopupNum = 0;
    private Image _panel;
    
    private void Awake()
    {
        _subcribedPopupUIEventSO.EventChannel += SubcribePopupUI;
        PopupUIEvent.displayPopupAction += DisplayPopup;

        _panel = GetComponent<Image>();
        ShowPanel(false);

    }
    private void OnDestroy()
    {
        _subcribedPopupUIEventSO.EventChannel -= SubcribePopupUI;
        PopupUIEvent.displayPopupAction -= DisplayPopup;
    }
    
    private void SubcribePopupUI(PopupUIType type, PopupUIBase popupUI)
    {
        if (popupUI != null) popupUI.HidePopup();
        _popupUIDics[type] = popupUI;
    }

    private void DisplayPopup(PopupUIType type, bool active = true)
    {
        _popupRequest.Enqueue(type);

        if (!_lockPopupHandler) _ = HandlePopupRequest(active);

    }
    private async UniTask HandlePopupRequest(bool active = true)
    {
        _lockPopupHandler = true;
        while (_popupRequest.TryDequeue(out var type))
        {
            if (_popupUIDics.TryGetValue(type, out var popupUI))
            {
                if (popupUI == null)
                {
                    Debug.LogWarning(type + " is null.");
                    break;
                }

                if (active)
                {
                    popupUI.ShowPopup();
                    if (_countPopupNum++ == 0) ShowPanel();
                }
                else
                {
                    popupUI.HidePopup();
                    if (_countPopupNum-- == 0) ShowPanel(false);
                }
            }
            else
            {
                Debug.LogWarning(type + " does not exist.");
            }
        }
        _lockPopupHandler = false;
        await UniTask.WaitForEndOfFrame();

    }
    private void ShowPanel(bool active = true)
    {
        if (_panel == null) return;
        _panel.enabled = active;
    }
}
