using UnityEngine;

[CreateAssetMenu(fileName = "JumpConfig", menuName = "Scriptable/PlayerConfig/JumpConfig")]
public class JumpConfig : ScriptableObject
{
    [Header("缓冲配置")]
    [Tooltip("跳跃缓冲时间（秒）")]
    public float jumpBufferDuration = 0.2f;

    [Header("起跳参数")]
    [Tooltip("起跳速度")]
    public float jumpForce = 15f;

    [Header("重力参数")]
    [Tooltip("按住跳跃键时的重力倍数（越小滞空越长）")]
    public float holdGravityScale = 2f;

    [Tooltip("松开跳跃键时的重力倍数（越大下落越快）")]
    public float releaseGravityScale = 5f;

    [Tooltip("默认重力（通常为1）")]
    public float defaultGravityScale = 1f;

    [Header("地面检测")]
    [Tooltip("地面检测层")]
    public LayerMask groundLayer = -1;
}