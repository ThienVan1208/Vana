using System;
using UnityEngine;


[CreateAssetMenu(fileName = "PopupUIEventSO", menuName = "EventChannel/PopupUIEventSO")]
public class PopupUIEventSO : ScriptableObject
{
    public Action<PopupUIType, PopupUIBase> EventChannel;
    public void RaiseEvent(PopupUIType arg1, PopupUIBase arg2)
    {
        EventChannel?.Invoke(arg1, arg2);
    }
}