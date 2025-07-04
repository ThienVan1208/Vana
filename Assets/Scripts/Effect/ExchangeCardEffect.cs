using System;
using UnityEngine;

public class ExchangeCardEffect : MonoBehaviour
{
    protected ObjectPooler<TextObject> _changeCardEffectPool;
    protected TextObject _textObj;
    [SerializeField] protected GameObject _textObjPrefab;

    protected virtual void Start()
    {
        _changeCardEffectPool = new ObjectPooler<TextObject>(_textObjPrefab, MainUICanvasEvent.RaiseGetMainUICanvasEvent().transform, 3);
        ObjectPoolManager.RegisterPool(this, _changeCardEffectPool);
    }
    protected virtual void OnDestroy()
    {
        ObjectPoolManager.RemovePoolObject(this);
    }
    public void GetEffect(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float size, Color color, float alpha = 1)
    {
        _textObj = _changeCardEffectPool.GetElem();
        _textObj.gameObject.SetActive(true);
        _textObj.DisplayText(timeDisplay, startPos, endPos, content, size, color, alpha, () =>
        {
            _textObj.gameObject.SetActive(false);
            _changeCardEffectPool.ReturnPool(_textObj);
        });
    }
}
