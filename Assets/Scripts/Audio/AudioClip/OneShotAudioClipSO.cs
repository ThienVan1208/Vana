using UnityEngine;
[CreateAssetMenu(fileName = "OneShotAudioClipSO", menuName = "AudioClipSO/OneShotAudioClipSO")]
public class OneShotAudioClipSO : AudioClipSO
{
    [SerializeField] private AudioClip _audioClip;
    public override AudioClip GetAudioClip()
    {
        return _audioClip;
    }

}
