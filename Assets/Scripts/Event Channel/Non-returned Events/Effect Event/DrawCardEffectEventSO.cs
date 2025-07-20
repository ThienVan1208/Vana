using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardEffectEventSO", menuName = "EventChannel/Effect/DrawCardEffectEventSO")]
public class DrawCardEffectEventSO : ScriptableObject
{
    public Action<float, Vector3, Vector3, string, float, Color, float> EventChannel;
    public void RaiseEvent(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float size, Color color, float alpha = 1)
    {
        EventChannel?.Invoke(timeDisplay, startPos, endPos, content, size, color, alpha);
    }
}