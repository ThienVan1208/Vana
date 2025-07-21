using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SequentialAudioClipSO", menuName = "AudioClipSO/SequentialAudioClipSO")]
public class SequentialAudioClipSO : AudioClipSO
{
    [SerializeField] private List<AudioClip> _audioClips= new List<AudioClip>();
    private int _curIdx = -1;

    public override AudioClip GetAudioClip()
    {
        _curIdx = (_curIdx + 1) % _audioClips.Count;
        return _audioClips[_curIdx];
    }

    
}
