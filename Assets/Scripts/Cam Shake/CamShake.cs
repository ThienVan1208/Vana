using DG.Tweening;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    [SerializeField] private CamShakeEventSO _camShakeEventSO;
    private void OnEnable() {
        _camShakeEventSO.EventChannel += GetCamShake;
    }
    private void OnDisable() {
        _camShakeEventSO.EventChannel -= GetCamShake;
    }
    private void GetCamShake(float strength, float duration)
    {
        // transform.DOShakePosition(duration, strength);
        transform.DOShakeRotation(duration, strength);
    }
}
