using System;
using UnityEngine;


[CreateAssetMenu(fileName = "VoidEventSO", menuName = "EventChannel/VoidEventSO", order = 0)]
public class VoidEventSO : ScriptableObject
{
    public Action EventChannel;
    public void RaiseEvent()
    {
        EventChannel?.Invoke();
    }
}