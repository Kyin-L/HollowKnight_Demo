using UnityEngine;

public class Crawler : Enemy
{
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Collider2D wallDetectCollider;
    [SerializeField] protected Collider2D groundDetectCollider;

    protected override void Start()
    {
        base.Start();
        context.Set("facingDirection", transform.localScale.x > 0 ? 1 : -1);
        context.Set("moveSpeed", enemyConfig.moveSpeed);
        context.Set("wallLayer", wallLayer);
        context.Set("groundLayer", groundLayer);
        context.Set("wallDetectCollider", wallDetectCollider);
        context.Set("groundDetectCollider", groundDetectCollider);
    }

    public override void TakeDamage(DamageInfo info)
    {
        if (isDead)
            return;

        if(info.force.HasValue)
            context.Set("isHurt", true);

        health.TakeDamage(info);
        SpawnEffect(info);
    }

    protected override void OnDead(DamageInfo info)
    {
        base.OnDead(info);
        GetComponent<AudioSource>().enabled = false;
    }
}
