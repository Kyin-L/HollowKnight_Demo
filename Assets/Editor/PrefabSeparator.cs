using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PrefabSeparator : EditorWindow
{
    [MenuItem("GameObject/分离子物体中的预制件", false, 0)]
    static void ShowWindow()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("请先选中一个父物体");
            return;
        }

        PrefabSeparatorWindow window = CreateInstance<PrefabSeparatorWindow>();
        window.parentObject = selected;
        window.ShowUtility();
    }
}

public class PrefabSeparatorWindow : EditorWindow
{
    public GameObject parentObject;
    private bool keepOriginalPrefabConnection = false;
    private bool recursiveSearch = true;
    private bool keepEmptyParents = false;
    private Vector2 scrollPosition;

    private List<PrefabInfo> prefabList;

    class PrefabInfo
    {
        public GameObject instance;
        public string prefabName;
        public Transform originalParent;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public int siblingIndex;
        public bool isPrefab;
    }

    void OnGUI()
    {
        if (parentObject == null)
        {
            EditorGUILayout.HelpBox("请先选中一个父物体", MessageType.Warning);
            return;
        }

        GUILayout.Label($"分离预制件: {parentObject.name}", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 扫描并显示统计
        if (GUILayout.Button("扫描预制件", GUILayout.Height(25)))
        {
            ScanPrefabs();
        }

        GUILayout.Space(5);

        // 选项
        recursiveSearch = EditorGUILayout.Toggle("递归搜索所有子物体", recursiveSearch);
        keepOriginalPrefabConnection = EditorGUILayout.Toggle("保持预制件连接（不分离）", keepOriginalPrefabConnection);
        keepEmptyParents = EditorGUILayout.Toggle("保留空父物体", keepEmptyParents);

        GUILayout.Space(10);

        // 显示预制件列表
        if (prefabList != null && prefabList.Count > 0)
        {
            EditorGUILayout.LabelField($"找到 {prefabList.Count} 个预制件实例:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            for (int i = 0; i < prefabList.Count; i++)
            {
                var prefab = prefabList[i];
                EditorGUILayout.BeginHorizontal();

                // 选择框
                prefab.isPrefab = EditorGUILayout.Toggle(prefab.isPrefab, GUILayout.Width(20));

                // 预制件信息
                string path = GetRelativePath(parentObject.transform, prefab.instance.transform);
                EditorGUILayout.LabelField($"{prefab.instance.name}", GUILayout.Width(150));
                EditorGUILayout.LabelField($"路径: {path}", EditorStyles.miniLabel);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            // 全选/取消按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("全选"))
            {
                foreach (var prefab in prefabList)
                    prefab.isPrefab = true;
            }
            if (GUILayout.Button("取消全选"))
            {
                foreach (var prefab in prefabList)
                    prefab.isPrefab = false;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 执行按钮
            int selectedCount = prefabList.Count(p => p.isPrefab);
            if (GUILayout.Button($"分离选中的预制件 ({selectedCount})", GUILayout.Height(35)))
            {
                SeparateSelectedPrefabs();
                Close();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("点击\"扫描预制件\"查找预制件实例", MessageType.Info);
        }
    }

    void ScanPrefabs()
    {
        if (parentObject == null) return;

        prefabList = new List<PrefabInfo>();

        // 获取所有要检查的物体
        IEnumerable<Transform> objectsToCheck;
        if (recursiveSearch)
        {
            objectsToCheck = parentObject.GetComponentsInChildren<Transform>(true);
        }
        else
        {
            objectsToCheck = parentObject.transform.Cast<Transform>();
        }

        foreach (Transform t in objectsToCheck)
        {
            // 跳过根物体
            if (t == parentObject.transform) continue;

            GameObject obj = t.gameObject;

            // 检查是否是预制件实例
            PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(obj);
            bool isPrefabInstance = assetType != PrefabAssetType.NotAPrefab &&
                                    PrefabUtility.GetPrefabInstanceStatus(obj) != PrefabInstanceStatus.NotAPrefab;

            if (isPrefabInstance)
            {
                PrefabInfo info = new PrefabInfo();
                info.instance = obj;
                info.prefabName = GetPrefabName(obj);
                info.originalParent = obj.transform.parent;
                info.localPosition = obj.transform.localPosition;
                info.localRotation = obj.transform.localRotation;
                info.localScale = obj.transform.localScale;
                info.siblingIndex = obj.transform.GetSiblingIndex();
                info.isPrefab = true;

                prefabList.Add(info);
            }
        }

        // 按层级深度排序
        prefabList = prefabList.OrderBy(p => GetDepth(p.instance.transform)).ToList();

        Debug.Log($"扫描完成，找到 {prefabList.Count} 个预制件实例");
    }

    string GetPrefabName(GameObject obj)
    {
        // 获取预制件资源路径
        GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(obj);
        if (prefabRoot != null)
        {
            return prefabRoot.name;
        }
        return obj.name;
    }

    string GetRelativePath(Transform root, Transform target)
    {
        if (target == root) return "";

        List<string> path = new List<string>();
        Transform current = target;

        while (current != null && current != root)
        {
            path.Insert(0, current.name);
            current = current.parent;
        }

        return string.Join("/", path);
    }

    int GetDepth(Transform t)
    {
        int depth = 0;
        while (t.parent != null && t.parent != parentObject.transform)
        {
            depth++;
            t = t.parent;
        }
        return depth;
    }

    void SeparateSelectedPrefabs()
    {
        if (parentObject == null) return;

        var selectedPrefabs = prefabList.Where(p => p.isPrefab).ToList();
        if (selectedPrefabs.Count == 0)
        {
            Debug.LogWarning("没有选中任何预制件");
            return;
        }

        // 按深度从深到浅排序（先处理深层物体）
        selectedPrefabs = selectedPrefabs.OrderByDescending(p => GetDepth(p.instance.transform)).ToList();

        Undo.RegisterFullObjectHierarchyUndo(parentObject, "Separate Prefabs");

        int successCount = 0;
        int failCount = 0;

        foreach (var prefabInfo in selectedPrefabs)
        {
            if (prefabInfo.instance == null) continue;

            try
            {
                if (keepOriginalPrefabConnection)
                {
                    // 保持预制件连接，只记录信息
                    Debug.Log($"保留预制件: {prefabInfo.instance.name}");
                    successCount++;
                    continue;
                }

                // 分离预制件：断开预制件连接但保留所有组件和数据
                GameObject newObject = SeparatePrefabInstance(prefabInfo);

                if (newObject != null)
                {
                    successCount++;
                    Debug.Log($"✅ 已分离: {prefabInfo.instance.name} → {newObject.name}");
                }
                else
                {
                    failCount++;
                    Debug.LogWarning($"⚠ 分离失败: {prefabInfo.instance.name}");
                }
            }
            catch (System.Exception e)
            {
                failCount++;
                Debug.LogError($"分离预制件 {prefabInfo.instance.name} 时出错: {e.Message}");
            }
        }

        // 清理空的父物体
        if (!keepEmptyParents)
        {
            CleanupEmptyParents();
        }

        // 刷新
        EditorUtility.SetDirty(parentObject);
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

        Debug.Log($"预制件分离完成！成功: {successCount}, 失败: {failCount}");

        if (successCount > 0)
        {
            EditorUtility.DisplayDialog("完成", $"成功分离 {successCount} 个预制件\n失败: {failCount}", "确定");
        }
    }

    GameObject SeparatePrefabInstance(PrefabInfo info)
    {
        GameObject original = info.instance;

        // 记录原始信息
        Transform parent = info.originalParent;
        Vector3 localPos = info.localPosition;
        Quaternion localRot = info.localRotation;
        Vector3 localScale = info.localScale;
        int siblingIdx = info.siblingIndex;

        // 方法1：使用 PrefabUtility.UnpackPrefabInstance（推荐）
        // 这可以断开预制件连接但保留所有组件和子物体
        PrefabUtility.UnpackPrefabInstance(original, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);

        // 验证是否成功断开
        PrefabAssetType newAssetType = PrefabUtility.GetPrefabAssetType(original);
        bool isStillPrefab = newAssetType != PrefabAssetType.NotAPrefab;

        if (!isStillPrefab)
        {
            // 成功断开，返回原物体
            return original;
        }
        else
        {
            // 如果断开失败，使用另一种方法
            Debug.LogWarning($"UnpackPrefabInstance 失败，尝试克隆方法: {original.name}");

            // 方法2：克隆并替换
            GameObject newObject = Instantiate(original, parent);
            newObject.name = original.name.Replace("(Clone)", "").Trim();
            newObject.transform.localPosition = localPos;
            newObject.transform.localRotation = localRot;
            newObject.transform.localScale = localScale;
            newObject.transform.SetSiblingIndex(siblingIdx);

            // 复制所有组件
            CopyComponents(original, newObject);

            // 删除原物体
            DestroyImmediate(original);

            return newObject;
        }
    }

    void CopyComponents(GameObject source, GameObject destination)
    {
        // 复制所有组件（Transform除外，因为已设置）
        Component[] components = source.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp is Transform) continue;

            UnityEditorInternal.ComponentUtility.CopyComponent(comp);
            Component targetComp = destination.GetComponent(comp.GetType());
            if (targetComp != null)
            {
                UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComp);
            }
            else
            {
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(destination);
            }
        }

        // 递归处理子物体
        for (int i = 0; i < source.transform.childCount; i++)
        {
            Transform sourceChild = source.transform.GetChild(i);
            Transform destChild = destination.transform.GetChild(i);

            if (destChild != null)
            {
                CopyComponents(sourceChild.gameObject, destChild.gameObject);
            }
        }
    }

    void CleanupEmptyParents()
    {
        List<GameObject> toDelete = new List<GameObject>();

        // 递归查找空物体
        FindEmptyObjects(parentObject.transform, toDelete);

        foreach (var empty in toDelete)
        {
            if (empty != null && empty.transform.parent != null)
            {
                Debug.Log($"删除空物体: {GetRelativePath(parentObject.transform, empty.transform)}");
                DestroyImmediate(empty);
            }
        }
    }

    void FindEmptyObjects(Transform current, List<GameObject> toDelete)
    {
        // 先处理子物体
        for (int i = current.childCount - 1; i >= 0; i--)
        {
            FindEmptyObjects(current.GetChild(i), toDelete);
        }

        // 检查当前物体是否为空
        if (current != parentObject.transform &&
            current.childCount == 0 &&
            current.GetComponent<Renderer>() == null &&
            current.GetComponent<Collider>() == null &&
            current.GetComponent<MeshFilter>() == null)
        {
            // 检查是否有其他重要组件
            Component[] components = current.GetComponents<Component>();
            if (components.Length <= 1) // 只有Transform
            {
                toDelete.Add(current.gameObject);
            }
        }
    }
}