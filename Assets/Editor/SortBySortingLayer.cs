using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class SortBySortingLayer : EditorWindow
{
    [MenuItem("GameObject/按SortingLayer分类子物体", false, 0)]
    static void ShowWindow()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("请先选中一个父物体");
            return;
        }

        SortBySortingLayerWindow window = CreateInstance<SortBySortingLayerWindow>();
        window.parentObject = selected;
        window.ShowUtility();
    }
}

public class SortBySortingLayerWindow : EditorWindow
{
    public GameObject parentObject;
    private bool createSubFolders = true;
    private bool keepOriginalOrder = true;
    private string subFolderPrefix = "Layer_";
    private Vector2 scrollPosition;

    private Dictionary<string, List<GameObject>> layerGroups;

    void OnGUI()
    {
        if (parentObject == null)
        {
            EditorGUILayout.HelpBox("请先选中一个父物体", MessageType.Warning);
            return;
        }

        GUILayout.Label($"按SortingLayer分类: {parentObject.name}", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 显示当前子物体统计
        int childCount = parentObject.transform.childCount;
        int rendererCount = parentObject.GetComponentsInChildren<SpriteRenderer>(true).Length;
        EditorGUILayout.LabelField($"当前子物体数: {childCount}");
        EditorGUILayout.LabelField($"SpriteRenderer数: {rendererCount}");

        GUILayout.Space(10);

        // 选项
        createSubFolders = EditorGUILayout.Toggle("创建分类父物体", createSubFolders);
        keepOriginalOrder = EditorGUILayout.Toggle("保持原有顺序", keepOriginalOrder);

        if (createSubFolders)
        {
            subFolderPrefix = EditorGUILayout.TextField("分类父物体前缀", subFolderPrefix);
        }

        GUILayout.Space(10);

        // 预览分类结果
        if (GUILayout.Button("预览分类", GUILayout.Height(25)))
        {
            PreviewClassification();
        }

        GUILayout.Space(5);

        // 显示预览
        if (layerGroups != null && layerGroups.Count > 0)
        {
            EditorGUILayout.LabelField("预览结果:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));

            foreach (var group in layerGroups)
            {
                EditorGUILayout.LabelField($"  📁 {group.Key}", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var obj in group.Value)
                {
                    EditorGUILayout.LabelField($"    • {obj.name}");
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(10);

        // 执行按钮
        if (GUILayout.Button("执行分类", GUILayout.Height(35)))
        {
            ExecuteClassification();
            Close();
        }
    }

    void PreviewClassification()
    {
        if (parentObject == null) return;

        layerGroups = new Dictionary<string, List<GameObject>>();

        // 获取所有直接子物体
        for (int i = 0; i < parentObject.transform.childCount; i++)
        {
            Transform child = parentObject.transform.GetChild(i);
            SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();

            if (renderer != null)
            {
                string layerName = renderer.sortingLayerName;
                if (!layerGroups.ContainsKey(layerName))
                    layerGroups[layerName] = new List<GameObject>();

                layerGroups[layerName].Add(child.gameObject);
            }
            else
            {
                // 没有SpriteRenderer的物体放到 "NoRenderer" 组
                if (!layerGroups.ContainsKey("无Renderer"))
                    layerGroups["无Renderer"] = new List<GameObject>();
                layerGroups["无Renderer"].Add(child.gameObject);
            }
        }

        // 按SortingLayer的渲染顺序排序组（可选）
        var sortedGroups = layerGroups.OrderBy(g => GetSortingLayerOrder(g.Key)).ToList();
        layerGroups = sortedGroups.ToDictionary(x => x.Key, x => x.Value);
    }

    int GetSortingLayerOrder(string layerName)
    {
        var layers = SortingLayer.layers;
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == layerName)
                return layers[i].value;
        }
        return 999; // 无Renderer组放最后
    }

    void ExecuteClassification()
    {
        if (parentObject == null) return;

        // 先获取预览数据
        PreviewClassification();

        if (layerGroups == null || layerGroups.Count == 0)
        {
            Debug.LogWarning("没有找到需要分类的子物体");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(parentObject, "Sort By SortingLayer");

        // 记录原始顺序信息
        Dictionary<GameObject, int> originalIndex = new Dictionary<GameObject, int>();
        if (keepOriginalOrder)
        {
            for (int i = 0; i < parentObject.transform.childCount; i++)
            {
                originalIndex[parentObject.transform.GetChild(i).gameObject] = i;
            }
        }

        // 为每个SortingLayer创建父物体
        Dictionary<string, GameObject> layerParents = new Dictionary<string, GameObject>();

        foreach (var group in layerGroups)
        {
            string layerName = group.Key;
            GameObject layerParent = null;

            if (createSubFolders)
            {
                // 创建或获取分类父物体
                string parentName = $"{subFolderPrefix}{layerName}";
                Transform existing = parentObject.transform.Find(parentName);

                if (existing != null)
                {
                    layerParent = existing.gameObject;
                    // 清空现有子物体
                    while (layerParent.transform.childCount > 0)
                    {
                        DestroyImmediate(layerParent.transform.GetChild(0).gameObject);
                    }
                }
                else
                {
                    layerParent = new GameObject(parentName);
                    layerParent.transform.SetParent(parentObject.transform);
                }

                layerParents[layerName] = layerParent;
            }

            // 移动子物体
            List<GameObject> objectsToMove = group.Value;

            if (keepOriginalOrder)
            {
                // 按原始顺序排序
                objectsToMove = objectsToMove.OrderBy(obj => originalIndex[obj]).ToList();
            }

            foreach (GameObject obj in objectsToMove)
            {
                if (createSubFolders && layerParent != null)
                {
                    obj.transform.SetParent(layerParent.transform);
                }
                else
                {
                    // 不创建父物体，只重新排序
                    // 按SortingLayer分组排序子物体
                    obj.transform.SetParent(parentObject.transform);
                }
            }
        }

        // 如果不创建父物体，则重新排列子物体顺序（按SortingLayer分组）
        if (!createSubFolders)
        {
            ReorderChildrenByLayer();
        }

        // 清理空的父物体
        if (createSubFolders)
        {
            CleanupEmptyParents();
        }

        EditorUtility.SetDirty(parentObject);
        Debug.Log($"✅ 分类完成！共分为 {layerGroups.Count} 个组，处理了 {layerGroups.Sum(g => g.Value.Count)} 个子物体");
    }

    void ReorderChildrenByLayer()
    {
        // 获取所有子物体并按SortingLayer分组排序
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < parentObject.transform.childCount; i++)
        {
            children.Add(parentObject.transform.GetChild(i));
        }

        // 按SortingLayer的渲染顺序排序，同组内保持原有顺序
        var sortedChildren = children
            .OrderBy(t =>
            {
                SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
                if (sr != null)
                    return GetSortingLayerOrder(sr.sortingLayerName);
                return 999;
            })
            .ThenBy(t => t.GetSiblingIndex())
            .ToList();

        // 重新设置顺序
        for (int i = 0; i < sortedChildren.Count; i++)
        {
            sortedChildren[i].SetSiblingIndex(i);
        }
    }

    void CleanupEmptyParents()
    {
        // 删除空的分类父物体
        for (int i = parentObject.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentObject.transform.GetChild(i);
            if (child.name.StartsWith(subFolderPrefix) && child.childCount == 0)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}