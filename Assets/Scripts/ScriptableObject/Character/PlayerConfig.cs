using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable/PlayerConfig/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("属性配置")]
    public int maxHealth = 5;
    public int maxSouls = 5;
    public float invincibilityDuration = 1f;
    public float hurtForce = 10f;

    [Header("移动配置")]
    public MoveConfig moveConfig;

    [Header("跳跃配置")]
    public JumpConfig jumpConfig;

    [Header("攻击配置")]
    public AttackConfig attackConfig;

    [Header("音效配置")]
    public PlayerAudioConfig audioConfig;
}