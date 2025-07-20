using System;
using UnityEngine;



[CreateAssetMenu(fileName = "TransformEventSO", menuName = "EventChannel/TransformEventSO", order = 0)]
public class TransformEventSO : ScriptableObject {
    public Action<Transform> EventChannel;
    public void RaiseEvent(Transform arg)
    {
        EventChannel?.Invoke(arg);
    }
}