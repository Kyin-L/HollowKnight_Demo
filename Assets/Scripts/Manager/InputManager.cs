using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager : SingletonBase<InputManager>
{ 
    private InputControl inputControl;
    private InputBuffer inputBuffer;
    public enum InputMode { None, Player, UI }
    private InputMode currentMode = InputMode.Player;

    public InputMode CurrentMode => currentMode;

    private InputManager()
    {
        inputControl = new InputControl();
        inputControl.Player.SetCallbacks(this);
        inputControl.UI.SetCallbacks(this);
        inputControl.Enable();
        SetInputMode(InputMode.Player);
    }

    public void SetInputBuffer(InputBuffer buffer)
    {
        inputBuffer = buffer;
    }

    public void SetInputMode(InputMode newMode)
    {
        if (currentMode == newMode) return;

        currentMode = newMode;

        switch (newMode)
        {
            case InputMode.Player:
                inputControl.Enable();
                inputControl.UI.Disable();
                inputControl.Player.Enable();
                break;
            case InputMode.UI:
                inputControl.Enable();
                inputControl.Player.Disable();
                inputControl.UI.Enable();
                break;
            case InputMode.None:
                inputControl.Disable();
                break;
        }
    }

    public void Enable()
    {
        inputControl.Enable();
    }

    public void Disable()
    {
        inputControl.Disable();
    }
}

public partial class InputManager : InputControl.IPlayerActions, InputControl.IUIActions
{
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            inputBuffer?.ReceiveAttack();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            inputBuffer?.ReceiveJumpStart();
        else if (context.canceled)
            inputBuffer?.ReceiveJumpCanceled();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            inputBuffer?.ReceiveMove(context.ReadValue<Vector2>());
        else if (context.canceled)
            inputBuffer?.ReceiveMoveCanceled();
    }
}
