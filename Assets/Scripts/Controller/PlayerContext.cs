using System;
using UnityEngine;

[Serializable]
public partial class PlayerContext
{
    //共享数据
    public bool isGround;
    public bool isDead;

    //角色数据
    public InputData inputData;
    public PlayerConfig playerConfig;
    public PlayerAudio playerAudio;

    //特效
    public ParticleSystem doubleJumpEffect;

    public ParticleSystem damagedEffect;
    public ParticleSystem ashEffect;
    public ParticleSystem shadeEffect;
}

public partial class PlayerContext
{
    //委托和事件
    //移动
    public Action<Vector2> onMove;
    public Action onMoveCanceled;
    public Action<float> addRecoil;
    public Action addUpRecoil;
    public Action<float> addDownRecoil;
    public Action<float> addDamagedForce;

    //跳跃
    public Action onJumpStart;
    public Action onJumpCanceled;

    //攻击
    public Action onAttack;
    public Func<bool> canAttack;

    public Action forwadSlash;
    public Action upSlash;
    public Action downSlash;

    //受伤
    public Action<DamageInfo> onDamaged;

    //死亡
    public Action onDead;

    public PlayerContext()
    {
        isDead = false;

        inputData = new();
    }
}