using TMPro;
using UnityEngine;

public class LoginUIPanel : UIBase
{
    [SerializeField] private TMP_InputField _accInput, _pwInput;
    private TouchScreenKeyboard _touchKeyboard; // Reference for mobile keyboard
    protected override void Awake()
    {
        base.Awake();
        ConfigureInputFieldsForMobile();
    }

    #region Config
    private bool IsMobilePlatform()
    {
        return Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
    private void OpenMobileKeyboard(TouchScreenKeyboardType keyboardType, bool secureInput = false)
    {
        if (IsMobilePlatform())
        {
            // Open the touchscreen keyboard with specified type.
            _touchKeyboard = TouchScreenKeyboard.Open(
                "", // Initial text (empty to avoid pre-filling).
                keyboardType,
                true, // Autocorrection (optional, set to false if unwanted).
                false, // Multiline (not needed for username/password).
                secureInput // Secure input for password.
            );
        }
    }
    private void ConfigureInputFieldsForMobile()
    {
        // Check if running on mobile platform.
        if (IsMobilePlatform())
        {
            // Ensure input fields trigger the correct keyboard type.
            _accInput.keyboardType = TouchScreenKeyboardType.Default;
            _pwInput.keyboardType = TouchScreenKeyboardType.Default; // Password uses secure input via contentType.

            // Add listeners to open keyboard explicitly when fields are selected.
            _accInput.onSelect.AddListener(_ => OpenMobileKeyboard(TouchScreenKeyboardType.Default));
            _pwInput.onSelect.AddListener(_ => OpenMobileKeyboard(TouchScreenKeyboardType.Default, true));
        }
    }
    #endregion

    #region Login
    // Used thru button.
    public void Login()
    {
        LoginEvent.RaiseLoginAction(_accInput.text, _pwInput.text);
    }

    #endregion
}
