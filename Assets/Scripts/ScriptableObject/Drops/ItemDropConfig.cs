using UnityEngine;

// 物品掉落（多一个itemId）
[CreateAssetMenu(fileName = "ItemDropConfig", menuName = "Scriptable/Drop/ItemConfig")]
public class ItemDropConfig : DropConfig
{
    public int itemId;  // 唯一的额外字段
}