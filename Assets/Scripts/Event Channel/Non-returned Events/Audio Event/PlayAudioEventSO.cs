using System;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayAudioEventSO", menuName = "EventChannel/Audio/PlayAudioEventSO")]
public class PlayAudioEventSO : ScriptableObject
{
    public Action<AudioClipSO> EventChannel;
    public void RaiseEvent(AudioClipSO audio)
    {
        EventChannel?.Invoke(audio);
    }
}