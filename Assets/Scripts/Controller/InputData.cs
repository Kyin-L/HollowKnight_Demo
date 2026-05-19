using UnityEngine;

public class InputData
{
    public bool enable = true;

    private Vector2 vectorInput = Vector2.zero;
    private bool attackInput;
    private bool jumpInput;

    public Vector2 VectorInput { set { vectorInput = value; } get { return enable ? vectorInput : Vector2.zero; } }
    public bool AttackInput { set { attackInput = value; } get { return enable ? attackInput : false; } }
    public bool JumpInput { set { jumpInput = value; } get { return enable ? jumpInput : false; } }

    public void Reset()
    {
        vectorInput = Vector2.zero;
        attackInput = false;
        jumpInput = false;
    }
}
