using UnityEngine;

public class CardPSEffect : MonoBehaviour
{
    private ObjectPooler<ParticleSystem> _psPool;
    private ParticleSystem _curPSEffect;
    private int initNum = 3;
    [SerializeField] private GameObject _glowEffectPrefabs;

    // Ref in HandHolder class.
    [SerializeField] private TransformEventSO _getGlowEventSO;
    [SerializeField] private BoolEventSO _stopGlowEventSO;

    private void Start()
    {
        _psPool = new ObjectPooler<ParticleSystem>(_glowEffectPrefabs, transform, initNum);

    }
    private void OnEnable()
    {
        _getGlowEventSO.EventChannel += GetGlowEffect;
        _stopGlowEventSO.EventChannel += StopGlowEffect;
    }
    private void OnDisable()
    {
        _getGlowEventSO.EventChannel -= GetGlowEffect;
        _stopGlowEventSO.EventChannel -= StopGlowEffect;
    }

    private void GetGlowEffect(Transform pos)
    {
        _curPSEffect = _psPool.GetElem();
        if (_curPSEffect == null) return;

        _curPSEffect.gameObject.SetActive(true);
        _curPSEffect.transform.SetParent(pos, false);
        _curPSEffect.transform.localPosition = new Vector3(-2.5f, 0f, 0f);
        _curPSEffect.transform.localScale = new Vector3(3.5f, 2.8f, 3.5f);

    }
    private void StopGlowEffect(bool isInactive)
    {
        _curPSEffect.Stop();
        if (isInactive) _curPSEffect.gameObject.SetActive(false);
        _psPool.ReturnPool(_curPSEffect);
    }
}
