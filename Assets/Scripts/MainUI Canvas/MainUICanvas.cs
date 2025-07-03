using UnityEngine;
using System;
public static class MainUICanvasEvent
{
    public static Func<Canvas> GetMainUICanvasEvent;
    public static Canvas RaiseGetMainUICanvasEvent()
    {
        return GetMainUICanvasEvent?.Invoke();
    }
}
public class MainUICanvas : MonoBehaviour
{
    private Canvas _mainUICanvas;
    private void Awake()
    {
        _mainUICanvas = GetComponent<Canvas>();
        if (_mainUICanvas == null)
        {
            Debug.LogError("Playable Canvas does not exist!!!");
            return;
        }
        MainUICanvasEvent.GetMainUICanvasEvent += () => _mainUICanvas;
    }
}
