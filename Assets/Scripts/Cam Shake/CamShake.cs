using System;
using DG.Tweening;
using UnityEngine;
public static class CamShakeEvent
{
    public static Action<float, float> CamShakeAction;
    public static void RaiseEvent(float arg1, float arg2)
    {
        CamShakeAction?.Invoke(arg1, arg2);
    }
}
public class CamShake : MonoBehaviour
{
    [SerializeField] private CamShakeEventSO _camShakeEventSO;
    private void OnEnable()
    {
        _camShakeEventSO.EventChannel += GetCamShake;
        CamShakeEvent.CamShakeAction += GetCamShake;
    }
    private void OnDisable()
    {
        _camShakeEventSO.EventChannel -= GetCamShake;
        CamShakeEvent.CamShakeAction -= GetCamShake;
    }
    private void GetCamShake(float strength, float duration)
    {
        // transform.DOShakePosition(duration, strength);
        transform.DOShakeRotation(duration, strength);
    }
}
