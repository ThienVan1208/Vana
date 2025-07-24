using UnityEngine;
public abstract class AudioClipSO : ScriptableObject
{
    [Range(0f, 1f)]
    public float vol = 1f;
    public abstract AudioClip GetAudioClip();
}