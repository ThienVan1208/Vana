using System;
using UnityEngine;


[CreateAssetMenu(fileName = "IntEventSO", menuName = "EventChannel/IntEventSO", order = 0)]
public class IntEventSO : ScriptableObject
{
    public Action<int> EventChannel;
    public void RaiseEvent(int arg)
    {
        EventChannel?.Invoke(arg);
    }
}