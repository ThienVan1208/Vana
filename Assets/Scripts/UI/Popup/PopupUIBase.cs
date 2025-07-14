using System;
using UnityEngine;


public class PopupUIBase : MonoBehaviour
{
    [SerializeField] protected PopupUIType popupUIType;
    [SerializeField] protected PopupUIEventSO subcribedPopupUIEventSO;
    [SerializeField] protected GameObject popupWindow;
    protected virtual void Awake() { }
    protected virtual void OnEnable()
    {
        subcribedPopupUIEventSO.RaiseEvent(popupUIType, this);
    }
    protected virtual void OnDisable()
    {
        subcribedPopupUIEventSO.RaiseEvent(popupUIType, null);
    }
    public virtual void ShowPopup(Action callback = null) { }
    public virtual void HidePopup(Action callback = null) { }
    public virtual void ShowPopup()
    {
        ShowPopup(null);
    }
    public virtual void HidePopup()
    {
        HidePopup(null);
    }
}
