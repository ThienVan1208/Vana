using System;
using UnityEngine;



[CreateAssetMenu(fileName = "RetBoolEventSO", menuName = "EventChannel/RetBoolEventSO")]
public class RetBoolEventSO : ScriptableObject {
    public Func<bool> EventChannel;
    public bool RaiseEvent()
    {
        return EventChannel?.Invoke() ?? false;
    }
}