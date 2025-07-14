using System;
using TMPro;
using UnityEngine;

public class RegisterPopupUI : PopupUIBase
{
    [SerializeField] private TMP_InputField _accInput, _pwInput, _confirmPwInput;
    private UIEffectBase _uiEffect;

    protected override void Awake()
    {
        _uiEffect = popupWindow.GetComponent<UIEffectBase>();
    }
    public override void ShowPopup(Action callback = null)
    {
        base.ShowPopup(callback);
        _uiEffect.GetEffect(callback: () => callback?.Invoke());
        popupWindow.SetActive(true);
    }
    public override void HidePopup(Action callback = null)
    {
        base.HidePopup(callback);
        _uiEffect.GetEffect(callback: () =>
        {
            callback?.Invoke();
            popupWindow.SetActive(false);
        }, reverse: true);

        callback?.Invoke();
    }

    // Used thru button.
    public void Register()
    {
        if (_confirmPwInput.text != _pwInput.text)
        {
            Debug.LogWarning("Confirm password is incorrect.");
            return;
        }
        LoginEvent.RaiseRegisterAction(_accInput.text, _pwInput.text);
    }
}
