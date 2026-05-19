using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;  // 鼠标图片
    public Vector2 hotSpot = Vector2.zero;  // 点击热点（图片中心点）
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        // 设置自定义鼠标
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}