using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


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
    protected override void OnDisable()
    {
        try
        {
            base.OnDisable();

            // DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0f, _fadeOutWhenDisableDuration)
            // .OnComplete(() =>
            // {
            //     audioSource.volume = audioVolume; // Reset volume after fade out
            //     _playAudioLock = false;
            //     audioSource.Stop();
            // });
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("PlaySequentialAudioHandler OnDisable was cancelled.");
        }
    }
    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    protected override void PlayAudio(AudioClipSO audioClipSO)
    {
        base.PlayAudio(audioClipSO);

        _playAudioLock = true;
        PlaySequence(audioClipSO);

    }
    private async void PlaySequence(AudioClipSO audioClipSO)
    {
        try
        {
            while (_playAudioLock)
            {
                var clip = audioClipSO.GetAudioClip();
                _curClipLength = clip.length;
                audioSource.PlayOneShot(clip);
                await UniTask.Delay(TimeSpan.FromSeconds(_curClipLength), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }
        catch (OperationCanceledException)
        {
        }


    }
}
