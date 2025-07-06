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
    public virtual void ShowPopup() { }
    public virtual void HidePopup() { }
}
