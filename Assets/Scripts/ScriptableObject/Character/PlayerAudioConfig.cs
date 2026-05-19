using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAudioConfig", menuName = "Scriptable/PlayerConfig/PlayerAudioConfig")]
public class PlayerAudioConfig : ScriptableObject
{
    [Header("移动音效")]
    public AudioClip walk;
    public AudioClip run;

    [Header("跳跃音效")]
    public AudioClip jump;
    public AudioClip doubleJump;
    public AudioClip softLand;
    public AudioClip hardLand;

    [Header("攻击音效")]
    public AudioClip forwardSlash1;
    public AudioClip forwardSlash2;
    public AudioClip upSlash;
    public AudioClip downSlash;

    [Header("受伤音效")]
    public AudioClip damaged;
}