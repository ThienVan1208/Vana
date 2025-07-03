using UnityEngine;

public class DisplayedTextObjectEffect : MonoBehaviour
{
    protected ObjectPooler<TextObject> _changeCardEffectPool;
    protected TextObject _textObj;
    [SerializeField] protected GameObject _textObjPrefab;

    protected virtual void Start()
    {
        _changeCardEffectPool = new ObjectPooler<TextObject>(_textObjPrefab, transform, 10);
        ObjectPoolManager.RegisterPool(this, _changeCardEffectPool);
    }
    protected virtual void OnDestroy()
    {
        ObjectPoolManager.RemovePoolObject(this);
    }
}
