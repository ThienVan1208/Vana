using System;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextObject))]
public class TextObjectEffect : MonoBehaviour
{
    protected virtual void OnValidate()
    {
        gameObject.GetComponent<TextObject>().SetEffect(this);
    }
    public virtual void GetEffect(TextMeshProUGUI displayText, Vector3 startPos, Vector3 endPos, float duration, Action callback = null){}
}
