using System;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(fileName = "InteractInputReaderSO", menuName = "InteractInputReaderSO")]
public class InteractInputReaderSO : ScriptableObject, Interact.IControlActions
{
    private Interact _interactInput;
    public event Action<Vector2> MousePosAction = delegate { };
    private void OnEnable()
    {
        _interactInput = new Interact();
        _interactInput.Control.SetCallbacks(this);
        _interactInput.Control.Enable();
    }
    private void OnDisable() {
        _interactInput.Control.Disable();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        MousePosAction?.Invoke(context.ReadValue<Vector2>());
    }
}
