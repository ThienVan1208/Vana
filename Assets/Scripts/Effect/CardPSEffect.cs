using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CardPSEffect : MonoBehaviour
{
    private ObjectPooler<ParticleSystem> _psPool;
    private int initNum = 3;
    [SerializeField] private GameObject _glowEffectPrefabs;

    private void Awake()
    {
        _psPool = new ObjectPooler<ParticleSystem>(_glowEffectPrefabs, transform, initNum);
        ObjectPoolManager.Instance?.RegisterPool(this, _psPool);
    }
    // public void GetGlowEffect(Transform pos)
    // {
    //     var effect = _psPool.GetElem();
    //     effect.transform.position = pos.position;
    //     effect.gameObject.SetActive(true);
    //     effect.transform.SetParent(pos);
    // }
    public ParticleSystem GetGlowEffect(Transform pos)
    {
        var effect = _psPool.GetElem();
        
        effect.gameObject.SetActive(true);
        effect.transform.SetParent(pos, false);
        effect.transform.position = new Vector3(-2.5f, 0f, 0f);
        effect.transform.localScale = new Vector3(3.5f, 2.8f, 3.5f);
        return effect;
    }
}
