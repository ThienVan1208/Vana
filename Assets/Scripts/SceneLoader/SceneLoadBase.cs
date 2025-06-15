using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneLoadBase : MonoBehaviour
{
    private float _maxEffectTime = 0f;
    [SerializeField] private List<LoaderEffectBase> _loadEffectList = new List<LoaderEffectBase>();
    private void OnValidate()
    {
        _maxEffectTime = 0f; // Reset to ensure correct max calculation
        for (int i = 0; i < _loadEffectList.Count; i++)
        {
            if (_loadEffectList[i] != null) // Check for null
            {
                float effectTime = _loadEffectList[i].GetEffectTime();
                if (effectTime > _maxEffectTime)
                {
                    _maxEffectTime = effectTime;
                }
                // Debug.Log("Register");
            }
            else
            {
                // Debug.LogWarning($"Element at index {i} in _loadEffectList is null.");
            }
        }
    }
    public void StartLoading()
    {
        foreach (var loader in _loadEffectList)
        {
            HelpLoader(loader.StartLoading);
        }
    }
    public void EndLoading()
    {
        foreach (var loader in _loadEffectList)
        {
            HelpLoader(loader.EndLoading);
        }
    }

    public float GetEffectTime()
    {
        return _maxEffectTime;
    }

    public void InitLoader()
    {
        foreach (var loader in _loadEffectList)
        {
            loader.InitLoader();
        }
    }
    private async void HelpLoader(Action loading)
    {
        loading.Invoke();
        await UniTask.WaitForEndOfFrame();
    }
}
