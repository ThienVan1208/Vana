using System;
using UnityEngine;
using UnityEngine.Events;

public class PopupUIBase : UIBase
{
    [SerializeField] protected PopupUIType popupUIType;
    [SerializeField] protected PopupUIEventSO subcribedPopupUIEventSO;
    [SerializeField] protected GameObject popupWindow;
    protected virtual void Awake() { }
    protected override void Start()
    {
        base.Start();
        subcribedPopupUIEventSO.RaiseEvent(popupUIType, this);
    }
    protected virtual void OnDestroy()
    {
        subcribedPopupUIEventSO.RaiseEvent(popupUIType, null);
    }
    public virtual void ShowPopup() { }
    public virtual void HidePopup() { }
}
