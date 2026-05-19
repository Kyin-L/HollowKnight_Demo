#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolMovement))]
public class PatrolMovementEditor : Editor
{
    private void OnEnable()
    {
        PatrolMovement movement = (PatrolMovement)target;

        // 检查是否是刚添加的组件（范围还是默认值）
        if (movement.minBounds == new Vector2(-5, -3) && movement.maxBounds == new Vector2(5, 3))
        {
            // 自动设置为以物体为中心
            SetBoundsToCenter(movement);
        }
    }

    private void SetBoundsToCenter(PatrolMovement movement)
    {
        float halfWidth = (movement.maxBounds.x - movement.minBounds.x) / 2;
        float halfHeight = (movement.maxBounds.y - movement.minBounds.y) / 2;
        Vector3 center = movement.transform.position;

        movement.minBounds = new Vector2(center.x - halfWidth, center.y - halfHeight);
        movement.maxBounds = new Vector2(center.x + halfWidth, center.y + halfHeight);

        EditorUtility.SetDirty(movement);
    }

    private void OnSceneGUI()
    {
        PatrolMovement movement = (PatrolMovement)target;

        // 获取四个角点
        Vector3 bottomLeft = new Vector3(movement.minBounds.x, movement.minBounds.y, 0);
        Vector3 topRight = new Vector3(movement.maxBounds.x, movement.maxBounds.y, 0);
        Vector3 topLeft = new Vector3(movement.minBounds.x, movement.maxBounds.y, 0);
        Vector3 bottomRight = new Vector3(movement.maxBounds.x, movement.minBounds.y, 0);

        Handles.color = Color.green;

        // 绘制矩形
        Handles.DrawLine(bottomLeft, bottomRight);
        Handles.DrawLine(bottomRight, topRight);
        Handles.DrawLine(topRight, topLeft);
        Handles.DrawLine(topLeft, bottomLeft);

        Handles.DrawSolidRectangleWithOutline(
            new Vector3[] { bottomLeft, bottomRight, topRight, topLeft },
            new Color(0, 1, 0, 0.05f),
            Color.green
        );

        float handleSize = HandleUtility.GetHandleSize(Vector3.zero) * 0.1f;

        // 8个控制点
        Vector3[] handlePoints = new Vector3[]
        {
            bottomLeft, bottomRight, topRight, topLeft,
            new Vector3((bottomLeft.x + bottomRight.x) / 2, bottomLeft.y, 0),
            new Vector3((topLeft.x + topRight.x) / 2, topLeft.y, 0),
            new Vector3(bottomLeft.x, (bottomLeft.y + topLeft.y) / 2, 0),
            new Vector3(bottomRight.x, (bottomRight.y + topRight.y) / 2, 0)
        };

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < handlePoints.Length; i++)
        {
            var fmh_74_17_639139478552687441 = Quaternion.identity; Vector3 newPos = Handles.FreeMoveHandle(
                handlePoints[i],
                handleSize,
                Vector3.zero,
                Handles.DotHandleCap
            );

            if (newPos != handlePoints[i])
            {
                Undo.RecordObject(movement, "调整移动范围");

                switch (i)
                {
                    case 0:
                        movement.minBounds = new Vector2(newPos.x, newPos.y);
                        break;
                    case 1:
                        movement.minBounds = new Vector2(movement.minBounds.x, newPos.y);
                        movement.maxBounds = new Vector2(newPos.x, movement.maxBounds.y);
                        break;
                    case 2:
                        movement.maxBounds = new Vector2(newPos.x, newPos.y);
                        break;
                    case 3:
                        movement.minBounds = new Vector2(newPos.x, movement.minBounds.y);
                        movement.maxBounds = new Vector2(movement.maxBounds.x, newPos.y);
                        break;
                    case 4:
                        movement.minBounds = new Vector2(movement.minBounds.x, newPos.y);
                        break;
                    case 5:
                        movement.maxBounds = new Vector2(movement.maxBounds.x, newPos.y);
                        break;
                    case 6:
                        movement.minBounds = new Vector2(newPos.x, movement.minBounds.y);
                        break;
                    case 7:
                        movement.maxBounds = new Vector2(newPos.x, movement.maxBounds.y);
                        break;
                }

                // 确保 min < max
                float minX = Mathf.Min(movement.minBounds.x, movement.maxBounds.x);
                float maxX = Mathf.Max(movement.minBounds.x, movement.maxBounds.x);
                float minY = Mathf.Min(movement.minBounds.y, movement.maxBounds.y);
                float maxY = Mathf.Max(movement.minBounds.y, movement.maxBounds.y);

                movement.minBounds = new Vector2(minX, minY);
                movement.maxBounds = new Vector2(maxX, maxY);

                EditorUtility.SetDirty(movement);
                break;
            }
        }

        // 显示信息
        Handles.BeginGUI();
        Vector3 center = new Vector3(
            (movement.minBounds.x + movement.maxBounds.x) / 2,
            movement.maxBounds.y + 0.3f,
            0
        );
        Vector2 screenPos = HandleUtility.WorldToGUIPoint(center);
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.green;
        style.fontSize = 10;

        float width = movement.maxBounds.x - movement.minBounds.x;
        float height = movement.maxBounds.y - movement.minBounds.y;
        GUI.Label(new Rect(screenPos.x - 60, screenPos.y - 15, 120, 20),
            $"范围: {width:F1} x {height:F1}", style);
        Handles.EndGUI();
    }
}
#endif