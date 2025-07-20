using System;
using UnityEngine;

public class DrawCardEffect : MonoBehaviour
{
    protected ObjectPooler<TextObject> _drawCardEffectPool;
    protected TextObject _textObj;
    [SerializeField] protected GameObject textObjPrefab;

    // Ref in InGamePanel class.
    [SerializeField] protected DrawCardEffectEventSO drawCardEffectEventSO;

    protected virtual void Start()
    {
        _drawCardEffectPool = new ObjectPooler<TextObject>(textObjPrefab, MainUICanvasEvent.RaiseGetMainUICanvasEvent().transform, 3);

    }

    private void OnEnable() {
        drawCardEffectEventSO.EventChannel += GetEffect;
    }
    private void OnDisable()
    {
        drawCardEffectEventSO.EventChannel -= GetEffect;
    }
    private void GetEffect(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float size, Color color, float alpha = 1)
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
