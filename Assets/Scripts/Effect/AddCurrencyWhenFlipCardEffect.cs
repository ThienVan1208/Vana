using System;
using UnityEngine;

public class AddCurrencyWhenFlipCardEffect : MonoBehaviour
{
    private ObjectPooler<TextObject> _pool;
    [SerializeField] private GameObject _prefab;
    protected TextObject _textObj;
    private void Start()
    {
        _pool = new ObjectPooler<TextObject>(_prefab, PlayableCanvasEvent.RaiseGetPlayableCanvasEvent().transform as RectTransform, 4);
        ObjectPoolManager.RegisterPool(this, _pool);
    }
    private void OnDestroy()
    {
        ObjectPoolManager.RemovePoolObject(this);
    }
    public GameObject GetEffect(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float fontSize, Color color, float alpha = 1, Action callback = null)
    {
        _textObj = _pool.GetElem();
        _textObj.gameObject.SetActive(true);
        _textObj.DisplayText(timeDisplay, startPos, endPos, content, fontSize, color, alpha, () =>
        {
            callback?.Invoke();
            _textObj.gameObject.SetActive(false);
            _pool.ReturnPool(_textObj);
        });
        return _textObj.gameObject;
    }
}
