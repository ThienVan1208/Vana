using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private AudioClipSO _audioClipSO;
    [SerializeField] private PlayAudioEventSO _playAudioEventSO;
    public bool triggerOnStart = false, triggerOnEnable = false, triggerOnDisable = false, triggerOnDestroy = false;
    private void Start()
    {
        if (triggerOnStart)
        {
            _playAudioEventSO.RaiseEvent(_audioClipSO);
        }
    }
    private void OnEnable()
    {
        if (triggerOnEnable)
        {
            _playAudioEventSO.RaiseEvent(_audioClipSO);
        }
    }
    private void OnDisable()
    {
        if (triggerOnDisable)
        {
            _playAudioEventSO.RaiseEvent(_audioClipSO);
        }
    }
    private void OnDestroy()
    {
        if (triggerOnDestroy)
        {
            _playAudioEventSO.RaiseEvent(_audioClipSO);
        }
    }

    // Used by buttons.
    public void TriggerAudio()
    {
        _playAudioEventSO.RaiseEvent(_audioClipSO);
    }
}
