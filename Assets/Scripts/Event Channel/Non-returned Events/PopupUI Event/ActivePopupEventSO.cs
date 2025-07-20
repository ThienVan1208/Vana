using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ActivePopupEventSO", menuName = "EventChannel/ActivePopupEventSO", order = 0)]
public class ActivePopupEventSO : ScriptableObject {
        public Action<PopupUIType, bool> EventChannel;
    public void RaiseEvent(PopupUIType type, bool active = true)
    {
        EventChannel?.Invoke(type, active);
    }
}