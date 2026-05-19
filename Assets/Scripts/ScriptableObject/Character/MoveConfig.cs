using UnityEngine;

[CreateAssetMenu(fileName = "MoveConfig", menuName = "Scriptable/PlayerConfig/MoveConfig")]
public class MoveConfig : ScriptableObject
{
    [Header("移动参数")]
    [Tooltip("移动速度")]
    public float moveSpeed = 8f;
}