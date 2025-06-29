using System;
using UnityEngine;
public static class PlayableCanvasEvent
{
    public static Func<Canvas> GetPlayableCanvasEvent;
    public static Canvas RaiseGetPlayableCanvasEvent()
    {
        return GetPlayableCanvasEvent?.Invoke();
    }
}
public class PlayableCanvas : MonoBehaviour
{
    private Canvas _playableCanvas;
    private void Awake()
    {
        _playableCanvas = GetComponent<Canvas>();
        if (_playableCanvas == null)
        {
            Debug.LogError("Playable Canvas does not exist!!!");
            return;
        }
        PlayableCanvasEvent.GetPlayableCanvasEvent += () => _playableCanvas;
    }
}
