using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfig", menuName = "Scriptable/PlayerConfig/AttackConfig")]
public class AttackConfig : ScriptableObject
{
    [Header("缓冲配置")]
    [Tooltip("攻击缓冲时间（秒）")]
    public float attackBufferDuration = 0.2f;

    [Header("攻击参数")]
    [Tooltip("攻击动画时长（秒）")]
    public float attackDuration = 0.4f;

    [Tooltip("攻击伤害")]
    public int damage = 10;

    [Tooltip("攻击击退力度")]
    public float attackForce = 10f;

    [Tooltip("攻击反弹力度")]
    public float recoilForce = 250f;

    [Tooltip("向下攻击反弹速度")]
    public float downRecoilVelocity = 15f;

    [Header("连击设置")]
    [Tooltip("连击窗口（秒）")]
    public float comboWindow = 0.5f;

    [Tooltip("最大连击数")]
    public int maxCombo = 2;

    [Tooltip("击中特效")]
    public GameObject hitNormalEffect;

    [Tooltip("击中敌人特效")]
    public GameObject hitEnemyEffect;

    [Tooltip("击中尖刺特效")]
    public GameObject hitSpikeEffect;

    public ContactFilter2D enemyContactFilter;
}