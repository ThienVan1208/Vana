using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChosenCardEventSO", menuName = "EventChannel/ChosenCardEventSO", order = 0)]
public class ChosenCardEventSO : ScriptableObject
{
    public Action<List<Card>> EventChannel;
    public void RaiseEvent(List<Card> arg)
    {
        EventChannel?.Invoke(arg);
    }
}
