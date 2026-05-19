using UnityEngine;
using UnityEngine.Events;

public partial class JumpAbility : AbilityBase
{
    private JumpConfig jumpConfig;
    private InputData inputData;
    private ParticleSystem doubleJumpEffect;

    private bool jumpFlag = false;
    private bool doubleJumpFlag = true;

    private float fallTime = 0;

    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int doubleJumpHash = Animator.StringToHash("DoubleJump");
    private readonly int landHash = Animator.StringToHash("Land");
    private readonly int speedYHash = Animator.StringToHash("SpeedY");


    private UnityAction onUpdate;
    private UnityAction onFixedUpdate;

    public JumpAbility(PlayerController playerController, PlayerContext context) : base(playerController, context)
    {
        jumpConfig = context.playerConfig.jumpConfig;
        inputData = context.inputData;
        doubleJumpEffect = context.doubleJumpEffect;

        onUpdate = GroundUpdate;
        onFixedUpdate = GroundFixedUpdate;

        context.onJumpStart = OnJumpStart;
        context.onJumpCanceled = OnJumpCanceled;
    }

    public override void OnUpdate()
    {
        onUpdate?.Invoke();
    }

    public override void OnFixedUpdate()
    {
        onFixedUpdate?.Invoke();
    }

    public void OnJumpStart()
    {
        if(CanJump())
            inputData.JumpInput = true;
    }

    public void OnJumpCanceled()
    {
        inputData.JumpInput = false;
    }

    public bool CanJump()
    {
        return context.isGround || doubleJumpFlag;
    }

    private void GroundUpdate()
    {
        if (!jumpFlag && inputData.JumpInput)
        {
            jumpFlag = true;
            animator.SetTrigger(jumpHash);
            animator.ResetTrigger(landHash);
            context.playerAudio.PlayOneShotJump();
            onUpdate = JumpUpdate;
        }
        else if(rb.velocity.y < 0)
        {
            onUpdate = FallUpdate;
        }
    }

    private void JumpUpdate()
    {
        if (jumpFlag)
            return;

        if (rb.velocity.y < 0)
        {
            inputData.JumpInput = false;
            animator.SetFloat(speedYHash, rb.velocity.y);
            onUpdate = FallUpdate;
        }
    }

    private void FallUpdate()
    {
        fallTime += Time.deltaTime;

        if (rb.velocity.y == 0 && context.isGround)
        {
            doubleJumpFlag = true;
            animator.SetFloat(speedYHash, 0);
            onUpdate = GroundUpdate;

            if (fallTime > 0.1f)
            {
                animator.SetTrigger(landHash);
                context.playerAudio.PlayOneShotSoftLand();
            }
            fallTime = 0;
        }
        else
        {
            animator.SetFloat(speedYHash, rb.velocity.y);

            if (doubleJumpFlag && inputData.JumpInput)
            {
                doubleJumpFlag = false;
                jumpFlag = true;
                animator.SetTrigger(doubleJumpHash);
                context.playerAudio.PlayOneShotDoubleJump();
                doubleJumpEffect.Play();
                onUpdate = JumpUpdate;
            }
        }
    }

    private void GroundFixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = jumpConfig.releaseGravityScale;

            onFixedUpdate = FallFixedUpdate;
        }
        else if (jumpFlag)
        {
            jumpFlag = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpConfig.jumpForce);
            rb.gravityScale = jumpConfig.holdGravityScale;
            onFixedUpdate = JumpFixedUpdate;
        }
    }

    private void JumpFixedUpdate()
    {
        if (!inputData.JumpInput)
        {
            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = jumpConfig.releaseGravityScale;

            onFixedUpdate = FallFixedUpdate;
        }
    }

    private void FallFixedUpdate()
    {
        if(rb.velocity.y >= 0)
        {
            onFixedUpdate = GroundFixedUpdate;
        }
        else if (jumpFlag)
        {
            jumpFlag = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpConfig.jumpForce);
            rb.gravityScale = jumpConfig.holdGravityScale;
            onFixedUpdate = JumpFixedUpdate;
        }
    }
}