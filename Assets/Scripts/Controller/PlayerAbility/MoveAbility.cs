using System.Collections;
using UnityEngine;

public class MoveAbility : AbilityBase
{
    private Vector3 flippedScale = new Vector3(-1, 1, 1);
    private Transform transform;
    private MoveConfig moveConfig;
    private InputData inputData;

    private bool canMove = true;

    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private readonly int TurnBackHash = Animator.StringToHash("TurnBack");

    public MoveAbility(PlayerController playerController, PlayerContext context) : base(playerController, context)
    {
        transform = playerController.transform;
        moveConfig = context.playerConfig.moveConfig;
        inputData = context.inputData;

        context.onMove = OnMove;
        context.onMoveCanceled = OnMoveCanceled;
        context.addRecoil = AddRecoilForce;
        context.addUpRecoil = AddUpRecoilForce;
        context.addDownRecoil = AddDownRecoilForce;
        context.addDamagedForce = AddDamagedForce;
    }

    public override void OnUpdate()
    {
        UpdateAnimator();
        UpdateDirection();
    }

    public override void OnFixedUpdate()
    {
        if (canMove)
            rb.velocity = new Vector2(inputData.VectorInput.x * moveConfig.moveSpeed, rb.velocity.y);
    }

    public void OnMove(Vector2 direction)
    {
        inputData.VectorInput = direction;
    }

    public void OnMoveCanceled()
    {
        inputData.VectorInput = Vector2.zero;
    }

    private void UpdateAnimator()
    {
        if (context.isGround)
        {
            animator.SetFloat(SpeedHash, Mathf.Abs(inputData.VectorInput.x));

            if (transform.localScale.x * inputData.VectorInput.x > 0)
            {
                animator.SetTrigger(TurnBackHash);
            }
        }
    }

    private void AddRecoilForce(float force)
    {
        playerController.StartCoroutine(AddBackRecoilForce(force));
    }

    private IEnumerator AddBackRecoilForce(float force)
    {
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(new Vector2(transform.localScale.x, 0) * force, ForceMode2D.Force);
        yield return new WaitForSeconds(0.33f);
        canMove = true;
    }

    private void AddUpRecoilForce()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void AddDownRecoilForce(float force)
    {
        inputData.JumpInput = false;
        rb.velocity = new Vector2(rb.velocity.x, context.playerConfig.attackConfig.downRecoilVelocity);
    }

    private void AddDamagedForce(float force)
    {
        playerController.StartCoroutine(IEAddDamagedForce(force));
    }

    private IEnumerator IEAddDamagedForce(float force)
    {
        canMove = false;
        rb.velocity = new Vector2(transform.localScale.x, 1) * force;
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.03f);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.3f);
        canMove = true;
    }

    private void UpdateDirection()
    {
        if (inputData.VectorInput.x == 0)
            return;

        if (inputData.VectorInput.x > 0)
        {
            transform.localScale = flippedScale;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}