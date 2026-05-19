using UnityEngine;

public class InputBuffer
{
    [Header("缓冲设置")]
    [SerializeField] private float attackBufferDuration = 0.05f;  // 攻击缓冲窗口

    public bool enable = false;

    private InputManager inputManager;
    private IMonoManager monoManager;
    private IInputReceiver inputReceiver;

    // 攻击缓冲
    private bool attackBuffer = false;
    private float attackBufferTime = -1f;

    public InputBuffer()
    {
        inputManager = InputManager.Instance;
        inputManager.SetInputBuffer(this);

        monoManager = MonoManager.Instance;
        monoManager.AddListener(Update);
    }

    public void SetReceiver(IInputReceiver receiver)
    {
        inputReceiver = receiver;
    }

    public void ReceiveMove(Vector2 direction)
    {
        inputReceiver?.OnMove(direction);
    }

    public void ReceiveMoveCanceled()
    {
        inputReceiver?.OnMoveCanceled();
    }

    public void ReceiveJumpStart()
    {
        //由于跳跃的有长短跳和二段跳的逻辑,因此不在这处理输入缓冲
        inputReceiver?.OnJumpStart();
    }

    public void ReceiveJumpCanceled()
    {
        inputReceiver?.OnJumpCanceled();
    }

    public void ReceiveAttack()
    {
        if (inputReceiver == null) 
            return;

        if (inputReceiver.CanAttack())
            ExecuteAttack();
        else
            BufferAttack();
    }

    private void BufferAttack()
    {
        attackBuffer = true;
        attackBufferTime = Time.time;
    }

    private void ExecuteAttack()
    {
        attackBuffer = false;
        inputReceiver?.OnAttack();
    }

    void Update()
    {
        if (inputReceiver == null) 
            return;

        // 处理攻击缓冲
        if (attackBuffer)
        {
            // 检查是否过期
            if (Time.time - attackBufferTime > attackBufferDuration)
            {
                attackBuffer = false;
            }
            // 检查是否可以执行
            else if (inputReceiver != null && inputReceiver.CanAttack())
            {
                ExecuteAttack();
            }
        }
    }
}