using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;


public class PlaySequentialAudioHandler : AudioSourceHandler
{
    private float _curClipLength;
    private bool _playAudioLock = false;
    private float _fadeOutWhenDisableDuration = 2f;
    protected override void Awake()
    {
        base.Awake();
        audioSource.playOnAwake = true;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        _playAudioLock = false;
        audioSource.volume = audioVolume;
    }
    protected override async void OnDisable()
    {
        base.OnDisable();

        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0f, _fadeOutWhenDisableDuration);
        await UniTask.Delay(TimeSpan.FromSeconds(_fadeOutWhenDisableDuration), cancellationToken: this.GetCancellationTokenOnDestroy());
        _playAudioLock = false;
        audioSource.Stop();
    }

    protected override void PlayAudio(AudioClipSO audioClipSO)
    {
        base.PlayAudio(audioClipSO);

        _playAudioLock = true;
        PlaySequence(audioClipSO);

    }
    private async void PlaySequence(AudioClipSO audioClipSO)
    {
        while (_playAudioLock)
        {
            var clip = audioClipSO.GetAudioClip();
            _curClipLength = clip.length;
            audioSource.PlayOneShot(clip);
            await UniTask.Delay(TimeSpan.FromSeconds(_curClipLength), cancellationToken: this.GetCancellationTokenOnDestroy());
        }

    }
}
