using System;
using UnityEngine;

public class DrawCardEffect : MonoBehaviour
{
    protected ObjectPooler<TextObject> _drawCardEffectPool;
    protected TextObject _textObj;
    [SerializeField] protected GameObject _textObjPrefab;

    protected virtual void Start()
    {
        _drawCardEffectPool = new ObjectPooler<TextObject>(_textObjPrefab, MainUICanvasEvent.RaiseGetMainUICanvasEvent().transform, 3);
        ObjectPoolManager.RegisterPool(this, _drawCardEffectPool);
    }
    protected virtual void OnDestroy()
    {
        ObjectPoolManager.RemovePoolObject(this);
    }
    public void GetEffect(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float size, Color color, float alpha = 1)
    {
        _textObj = _drawCardEffectPool.GetElem();
        _textObj.gameObject.SetActive(true);
        _textObj.DisplayText(timeDisplay, startPos, endPos, content, size, color, alpha, () =>
        {
            _textObj.gameObject.SetActive(false);
            _drawCardEffectPool.ReturnPool(_textObj);
        });
    }
}
