using UnityEngine;
[System.Serializable]
public abstract class LoaderEffectBase : MonoBehaviour
{
    public abstract void InitLoader();
    public abstract void StartLoading();
    public abstract void EndLoading();
    public abstract float GetEffectTime();
}
