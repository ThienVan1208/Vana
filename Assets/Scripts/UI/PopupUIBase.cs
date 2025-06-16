using UnityEngine;

public class PopupUIBase : MonoBehaviour
{
    [SerializeField] protected PopupUIType popupUIType;
    [SerializeField] protected PopupUIEventSO subcribedPopupUIEventSO;
    [SerializeField] protected GameObject popupWindow;
    protected virtual void Start()
    {
        subcribedPopupUIEventSO.RaiseEvent(popupUIType, this);
    }
    protected virtual void OnDestroy()
    {
        subcribedPopupUIEventSO.RaiseEvent(popupUIType, null);
    }
    public virtual void ShowPopup() { }
    public virtual void HidePopup() { }
}
