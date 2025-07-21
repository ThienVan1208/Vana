using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioSourceHandler : MonoBehaviour
{
    [SerializeField] protected PlayAudioEventSO playAudioEventSO;

    [Range(0f, 1f)]
    [SerializeField] protected float audioVolume = 1f;

    protected AudioSource audioSource;
    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    protected virtual void OnEnable()
    {
        AudioManager.MuteAction += SetMute;
        playAudioEventSO.EventChannel += PlayAudio;
    }
    protected virtual void OnDisable()
    {
        AudioManager.MuteAction -= SetMute;
        playAudioEventSO.EventChannel -= PlayAudio;
    }
    protected virtual void SetMute(bool isMute)
    {
        if (isMute)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = audioVolume;
        }
    }
    protected virtual void PlayAudio(AudioClipSO audioClipSO) { }
}
