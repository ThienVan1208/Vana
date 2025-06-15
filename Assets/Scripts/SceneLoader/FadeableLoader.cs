using UnityEngine;
using UnityEngine.UI;

public class FadeableLoader : LoaderEffectBase
{
    [SerializeField] private Image _fadeImg;
    private float _fadeTime = 1;

    public override void StartLoading()
    {
        UIEffectUtils.FadeEffect(_fadeImg, 1, _fadeTime);
    }
    public override void EndLoading()
    {
        UIEffectUtils.FadeEffect(_fadeImg, 0f, _fadeTime);
    }

    public override float GetEffectTime()
    {
        return _fadeTime;
    }

    public override void InitLoader()
    {
        UIEffectUtils.FadeEffect(_fadeImg, 0f, 0);
    }

}
