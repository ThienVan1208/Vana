using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEventSO", menuName = "EventChannel/CardEventSO")]
public class CardEventSO : ScriptableObject
{
    public Action<Card> EventChannel;
    public void RaiseEvent(Card arg)
    {
        EventChannel?.Invoke(arg);
    }
}
