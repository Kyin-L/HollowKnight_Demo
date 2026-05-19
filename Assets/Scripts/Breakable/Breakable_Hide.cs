public class Breakable_Hide : Breakable
{
    protected override void OnBreak(DamageInfo info)
    {
        OnDamage(info);
        poolManager.Push(gameObject);
    }
}
