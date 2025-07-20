using System;
using UnityEngine;

public class AddCurrencyWhenFlipCardEffect : MonoBehaviour
{
    private ObjectPooler<TextObject> _pool;
    [SerializeField] private GameObject _prefab;

    // Ref in RuleGameHandler class.
    [SerializeField] private CurrencyFlipCardEffectEventSO _currencyFlipCardEffectEventSO;
    protected TextObject _textObj;
    private void Start()
    {
        _pool = new ObjectPooler<TextObject>(_prefab, PlayableCanvasEvent.RaiseGetPlayableCanvasEvent().transform as RectTransform, 4);
        // ObjectPoolManager.RegisterPool(this, _pool);
    }
    private void OnDestroy()
    {
        // ObjectPoolManager.RemovePoolObject(this);
    }
    private void OnEnable() {
        _currencyFlipCardEffectEventSO.EventChannel += GetEffect;
    }
    public void GetEffect(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float fontSize, Color color, float alpha = 1, Transform parent = null, Action callback = null)
    {
        _textObj = _pool.GetElem();
        _textObj.gameObject.SetActive(true);
        if (parent != null) _textObj.transform.SetParent(parent);
        _textObj.DisplayText(timeDisplay, startPos, endPos, content, fontSize, color, alpha, () =>
        {
            callback?.Invoke();
            _textObj.gameObject.SetActive(false);
            _pool.ReturnPool(_textObj);
        });
    }
}
