using UnityEngine;

public abstract class AbilityBase : IAbility
{
    protected PlayerController playerController;
    protected PlayerContext context;
    protected Animator animator;
    protected Rigidbody2D rb;

    public AbilityBase(PlayerController playerController, PlayerContext context)
    {
        this.playerController = playerController;
        this.context = context;
        animator = playerController.GetComponent<Animator>();
        rb = playerController.GetComponent<Rigidbody2D>();
    }

    public abstract void OnFixedUpdate();

    public abstract void OnUpdate();
}
