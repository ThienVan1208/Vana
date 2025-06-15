using UnityEngine;

public interface ISceneLoadable
{
    public void InitLoader();
    public void StartLoading();
    public void EndLoading();
    public float GetEffectTime();
}
