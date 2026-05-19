using UnityEngine;
using UnityEditor;
using System.Linq;

public class SortingLayerTool : EditorWindow
{
    [MenuItem("GameObject/设置Sorting Layer和Order In Layer", false, 0)]
    static void ApplyToSelected()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("请先选择一个物体");
            return;
        }

        // 创建设置窗口
        SortingLayerWindow window = CreateInstance<SortingLayerWindow>();
        window.selectedObjects = selectedObjects;

        // 在显示窗口前先加载默认值
        window.LoadDefaultValues();

        // 显示窗口
        window.ShowUtility();
    }
}

public class SortingLayerWindow : EditorWindow
{
    public GameObject[] selectedObjects;
    private string[] sortingLayerNames;
    private int selectedLayerIndex = 0;
    private int orderInLayer = 0;
    private string previewInfo = "";

    // 公开方法，供外部调用
    public void LoadDefaultValues()
    {
        // 获取所有Sorting Layer名称
        sortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();

        if (selectedObjects == null || selectedObjects.Length == 0)
            return;

        GameObject firstObject = selectedObjects[0];

        // 获取默认Renderer
        Renderer defaultRenderer = GetDefaultRenderer(firstObject);

        if (defaultRenderer != null)
        {
            // 设置Sorting Layer
            string currentLayerName = defaultRenderer.sortingLayerName;
            int index = System.Array.IndexOf(sortingLayerNames, currentLayerName);
            if (index >= 0)
                selectedLayerIndex = index;
            else
                selectedLayerIndex = 0;

            // 设置Order In Layer
            orderInLayer = defaultRenderer.sortingOrder;

            // 生成预览信息
            string source = GetRendererSource(firstObject, defaultRenderer);
            previewInfo = $"默认值来自: {source}\nSorting Layer: {currentLayerName}\nOrder In Layer: {orderInLayer}";
        }
        else
        {
            previewInfo = $"⚠ 选中物体 \"{firstObject.name}\" 及其子物体中没有找到Renderer组件\n将使用默认值: SortingLayer=Default, Order=0";
            selectedLayerIndex = GetLayerIndex("Default");
            orderInLayer = 0;
        }

        // 添加统计信息
        CollectPreviewInfo();
    }

    Renderer GetDefaultRenderer(GameObject obj)
    {
        if (obj == null) return null;

        // 优先查找物体本身的Renderer
        Renderer selfRenderer = obj.GetComponent<Renderer>();
        if (selfRenderer != null)
            return selfRenderer;

        // 如果没有，查找子物体中的第一个Renderer
        Renderer[] childRenderers = obj.GetComponentsInChildren<Renderer>(true);
        if (childRenderers.Length > 0)
            return childRenderers[0];

        return null;
    }

    string GetRendererSource(GameObject root, Renderer renderer)
    {
        if (renderer.gameObject == root)
            return $"\"{root.name}\" 自身";

        // 获取相对路径
        string path = renderer.name;
        Transform current = renderer.transform.parent;

        while (current != null && current.gameObject != root)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return $"\"{root.name}\" 下的 \"{path}\"";
    }

    int GetLayerIndex(string layerName)
    {
        for (int i = 0; i < sortingLayerNames.Length; i++)
        {
            if (sortingLayerNames[i] == layerName)
                return i;
        }
        return 0;
    }

    void CollectPreviewInfo()
    {
        if (selectedObjects == null || selectedObjects.Length == 0)
            return;

        int totalRenderers = 0;
        int objectsWithNoRenderer = 0;

        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                objectsWithNoRenderer++;
            else
                totalRenderers += renderers.Length;
        }

        previewInfo += $"\n\n📊 统计: {selectedObjects.Length} 个物体, 共 {totalRenderers} 个Renderer";
        if (objectsWithNoRenderer > 0)
            previewInfo += $"\n⚠ 其中 {objectsWithNoRenderer} 个物体没有Renderer组件";
    }

    void OnGUI()
    {
        GUILayout.Label("批量设置Sorting Layer和Order In Layer", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(previewInfo, MessageType.Info);
        GUILayout.Space(10);

        // Sorting Layer下拉选择
        int newLayerIndex = EditorGUILayout.Popup("Sorting Layer", selectedLayerIndex, sortingLayerNames);
        if (newLayerIndex != selectedLayerIndex)
            selectedLayerIndex = newLayerIndex;

        // Order In Layer输入
        orderInLayer = EditorGUILayout.IntField("Order In Layer", orderInLayer);

        GUILayout.Space(10);

        // 显示将要影响的物体列表
        EditorGUILayout.LabelField("将影响的物体:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null) continue;

            int rendererCount = obj.GetComponentsInChildren<Renderer>(true).Length;
            if (rendererCount > 0)
                EditorGUILayout.LabelField($"{obj.name} ({rendererCount} 个Renderer)");
            else
                EditorGUILayout.LabelField($"{obj.name} ⚠ 无Renderer");
        }
        EditorGUI.indentLevel--;

        GUILayout.Space(15);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("应用", GUILayout.Height(30)))
        {
            ApplyToSelectedObjects(true);
            Close();
        }

        if (GUILayout.Button("只应用sorting", GUILayout.Height(30)))
        {
            ApplyToSelectedObjects(false);
            Close();
        }

        if (GUILayout.Button("取消", GUILayout.Height(30)))
        {
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }

    void ApplyToSelectedObjects(bool enableOrder)
    {
        string targetLayer = sortingLayerNames[selectedLayerIndex];
        int totalCount = 0;
        int objectsProcessed = 0;

        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null) continue;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);

            if (renderers.Length == 0)
            {
                Debug.LogWarning($"物体 \"{obj.name}\" 及其子物体中没有Renderer组件，已跳过");
                continue;
            }

            objectsProcessed++;

            foreach (Renderer renderer in renderers)
            {
                Undo.RecordObject(renderer, "Batch Set Sorting Layer");
                renderer.sortingLayerName = targetLayer;
                if (enableOrder)
                {
                    renderer.sortingOrder = orderInLayer;
                }
                EditorUtility.SetDirty(renderer);
                totalCount++;
            }
        }

        // 只输出到Console，不弹窗
        Debug.Log($"✅ 已完成! 处理了 {objectsProcessed}/{selectedObjects.Length} 个物体，共设置 {totalCount} 个Renderer的Sorting参数");

        // 如果没有找到任何Renderer，只在Console警告
        if (totalCount == 0)
        {
            Debug.LogWarning("没有找到任何Renderer组件，请确保选中的物体或其子物体包含Renderer（如SpriteRenderer、MeshRenderer等）");
        }
    }
}