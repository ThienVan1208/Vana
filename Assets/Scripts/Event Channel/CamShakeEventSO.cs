using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CamShakeEventSO", menuName = "EventChannel/CamShakeEventSO")]
public class CamShakeEventSO : ScriptableObject
{
    public Action<float, float> EventChannel;
    public void RaiseEvent(float arg1, float arg2)
    {
        EventChannel?.Invoke(arg1, arg2);
    }
}
