using System;

public class Health : IDamagable
{
    public int maxHp;
    public int hp;

    public Action<DamageInfo> onDamage;
    public Action<DamageInfo> onDead;

    public Health(int maxHp, Action<DamageInfo> onDamage, Action<DamageInfo> onDead)
    {
        this.maxHp = maxHp;
        hp = maxHp;
        this.onDamage = onDamage;
        this.onDead = onDead;
    }

    public void TakeDamage(DamageInfo info)
    {
        hp -= info.amount;
        if (hp > 0) 
        {
            onDamage?.Invoke(info);
        }
        else
        {
            onDead?.Invoke(info);
        }
    }

    public void Reset()
    {
        hp = maxHp;
    }
}
