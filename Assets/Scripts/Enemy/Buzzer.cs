using UnityEngine;
using EventHandler.Respawn;
public class Buzzer : Enemy
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected Collider2D playerDetectCollider;

    private IEventManager eventManager;

    protected override void Start()
    {
        base.Start();
        context.Set("flySpeed", enemyConfig.moveSpeed);
        context.Set("playerLayer", playerLayer);
        context.Set("playerDetectCollider", playerDetectCollider);

        eventManager = ManagerLocator.Get<IEventManager>();
        eventManager.AddListener<ScreenToBlackEventHandler>(RemoveTarget);
    }

    protected void OnDestroy()
    {
        eventManager.RemoveListener<ScreenToBlackEventHandler>(RemoveTarget);
    }

    public override void TakeDamage(DamageInfo info)
    {
        if (isDead)
            return;

        if (info.force.HasValue)
            context.Set("isHurt", true);

        health.TakeDamage(info);
        SpawnEffect(info);
    }

    protected override void OnDead(DamageInfo info)
    {
        base.OnDead(info);
        rb.gravityScale = 3f;
        GetComponent<AudioSource>().enabled = false;
    }

    private void RemoveTarget(ScreenToBlackEventHandler handler)
    {
        context.Remove("player");
    }
}
