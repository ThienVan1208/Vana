using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
public class TextObject : MonoBehaviour
{
    [SerializeField]private TextObjectEffect _textObjectEffect;
    [SerializeField] private TextMeshProUGUI _displayText;
    private void OnValidate()
    {
        _displayText = GetComponent<TextMeshProUGUI>();
    }
    public void DisplayText(float timeDisplay, Vector3 startPos, Vector3 endPos, string content, float size, Color color, float alpha = 1, Action callback = null)
    {
        _displayText.rectTransform.localScale = Vector3.one;
        _displayText.text = content;
        _displayText.fontSize = size;
        _displayText.color = color;
        _displayText.alpha = alpha;

        _textObjectEffect.GetEffect(_displayText, startPos, endPos, timeDisplay, callback);

    }
    public void SetEffect(TextObjectEffect effect)
    {
        _textObjectEffect = effect;
    }
}
