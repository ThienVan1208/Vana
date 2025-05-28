using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AddCard2PlayerEventSO", menuName = "EventChannel/AddCard2PlayerEventSO")]
public class AddCard2PlayerEventSO : ScriptableObject
{
    public Action<int, List<Card>> EventChannel;
    public void RaiseEvent(int playerIndex, List<Card> cards)
    {
        EventChannel?.Invoke(playerIndex, cards);
    }
}