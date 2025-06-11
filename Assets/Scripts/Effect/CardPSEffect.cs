using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CardPSEffect : MonoBehaviour
{
    private ObjectPooler<ParticleSystem> _psPool;
    private ParticleSystem _curPSEffect;
    private int initNum = 3;
    [SerializeField] private GameObject _glowEffectPrefabs;

    private void Start()
    {
        _psPool = new ObjectPooler<ParticleSystem>(_glowEffectPrefabs, transform, initNum);
        ObjectPoolManager.Instance?.RegisterPool(this, _psPool);
    }

    public ParticleSystem GetGlowEffect(Transform pos)
    {
        _curPSEffect = _psPool.GetElem();

        _curPSEffect.gameObject.SetActive(true);
        _curPSEffect.transform.SetParent(pos, false);
        _curPSEffect.transform.localPosition = new Vector3(-2.5f, 0f, 0f);
        _curPSEffect.transform.localScale = new Vector3(3.5f, 2.8f, 3.5f);
        return _curPSEffect;
    }
    public void StopGlowEffect(bool isInactive = false)
    {
        _curPSEffect.Stop();
        if (isInactive) _curPSEffect.gameObject.SetActive(false);
    }
}
