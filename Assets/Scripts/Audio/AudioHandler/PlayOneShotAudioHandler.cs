public class PlayOneShotAudioHandler : AudioSourceHandler
{
    protected override void Awake()
    {
        base.Awake();
        audioSource.volume = audioVolume;
        audioSource.playOnAwake = false;
    }
    protected override void PlayAudio(AudioClipSO audioClipSO)
    {
        base.PlayAudio(audioClipSO);
        audioSource.PlayOneShot(audioClipSO.GetAudioClip());
    }
}
