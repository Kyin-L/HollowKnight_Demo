using UnityEngine;

public interface IDamagable
{
    void TakeDamage(DamageInfo damageInfo);
}

public struct DamageInfo
{
    public int amount;
    public GameObject source;
    public Vector2? damagePoint;
    public Vector2? force;
    public bool respawnFlag;

    public DamageInfo(int amount, GameObject source, Vector2? damagePoint = null, Vector2? force = null, bool respawnFlag = false)
    {
        this.amount = amount;
        this.source = source;
        this.damagePoint = damagePoint;
        this.force = force;
        this.respawnFlag = respawnFlag;
    }

    public void Test()
    {
        new DamageInfo(1, new GameObject(), respawnFlag: false);
    }
}
