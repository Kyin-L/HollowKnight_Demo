using UnityEngine;

public interface IInputReceiver
{
    void OnMove(Vector2 direction);
    void OnMoveCanceled();

    void OnJumpStart();
    void OnJumpCanceled();

    void OnAttack();

    bool CanAttack();

    public void ControlEnable();

    public void ControlDisable(float time = 0);
}
