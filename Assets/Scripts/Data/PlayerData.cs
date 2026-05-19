using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    //需要存储的数据
    private int maxHp;
    private int maxSouls;
    private int attackDamage;
    private int geo;

    //不需要存储的数据
    [NonSerialized]
    private int hp;
    [NonSerialized]
    private int souls;

    [NonSerialized]
    private IEventManager eventManager;

    public int HP
    {
        set 
        { 
            hp = Mathf.Clamp(value, 0, maxHp);
            eventManager.EventTrigger(EventHandler.PlayerData.HpChangedEventHandler.Get(hp));
        }
        get { return hp; }
    }

    public int Souls
    {
        set
        {
            souls = Mathf.Clamp(value, 0, maxSouls);
            eventManager.EventTrigger(EventHandler.PlayerData.SoulChangedEventHandler.Get(souls));
        }
        get { return souls; }
    }

    public int Geo
    {
        set
        {
            geo = value;
            eventManager.EventTrigger(EventHandler.PlayerData.GeoChangedEventHandler.Get(geo));
        }
        get { return geo; }
    }

    public PlayerData(PlayerConfig playerConfig)
    {
        eventManager = ManagerLocator.Get<IEventManager>();
        maxHp = playerConfig.maxHealth;
        maxSouls = playerConfig.maxSouls;
        HP = maxHp;
        Souls = 0;
        Geo = 0;
    }

    public void Respawn()
    {
        HP = maxHp;
        Souls = 0;
    }
}
