using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyFlipCardEffectEventSO", menuName = "EventChannel/Effect/CurrencyFlipCardEffectEventSO")]
public class CurrencyFlipCardEffectEventSO : ScriptableObject
{
    public Action<float, Vector3, Vector3, string, float, Color, float, Transform, Action> EventChannel;
    public void RaiseEvent(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float fontSize, Color color, float alpha = 1, Transform parent = null, Action callback = null)
    {
        EventChannel?.Invoke(timeDisplay, startPos, endPos, content, fontSize, color, alpha, parent, callback);
    }
}
