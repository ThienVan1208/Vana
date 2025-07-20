using System;
using UnityEngine;


[CreateAssetMenu(fileName = "TestSO", menuName = "TestSO", order = 0)]
public class TestSO<T> : ScriptableObject
{
    public static Func<bool> EventChannel;
    public static bool RaiseEvent()
    {
        return EventChannel?.Invoke() ?? false;
    }
}