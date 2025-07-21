using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RandomAudioClipSO", menuName = "AudioClipSO/RandomAudioClipSO")]
public class RandomAudioClipSO : AudioClipSO
{
    [SerializeField] private List<AudioClip> _audioClips= new List<AudioClip>();

    public override AudioClip GetAudioClip()
    {
        return _audioClips[Random.Range(0, _audioClips.Count)];
    }

    
}
