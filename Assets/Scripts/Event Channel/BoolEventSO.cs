using System;
using UnityEngine;


[CreateAssetMenu(fileName = "IntEventSO", menuName = "EventChannel/BoolEventSO")]
public class BoolEventSO : ScriptableObject
{
    public Action<bool> EventChannel;
    public void RaiseEvent(bool arg)
    {
        EventChannel?.Invoke(arg);
    }
}