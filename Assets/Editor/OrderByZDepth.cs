using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class OrderByZDepth : EditorWindow
{
    [MenuItem("GameObject/根据Z深度设置OrderInLayer", false, 0)]
    static void ShowWindow()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("请先选中一个父物体");
            return;
        }

        OrderByZDepthWindow window = CreateInstance<OrderByZDepthWindow>();
        window.parentObject = selected;
        window.ShowUtility();
    }
}

public class OrderByZDepthWindow : EditorWindow
{
    public GameObject parentObject;

    private bool useWorldPosition = false;
    private bool recursiveSearch = true;
    private int orderStart = 0;
    private int orderStep = 1;
    private bool reverseOrder = false;
    private bool includeInactive = true;
    private string targetSortingLayer = "";
    private bool resetZToZero = true;  // 新增：是否将Z轴归零
    private bool resetLocalPosition = true;  // 新增：重置局部坐标还是世界坐标

    private Vector2 scrollPosition;
    private List<RendererInfo> rendererList;
    private string[] sortingLayerNames;
    private int selectedLayerIndex = 0;

    class RendererInfo
    {
        public Renderer renderer;
        public float zDepth;
        public int originalOrder;
        public string path;
        public int newOrder;
        public Vector3 originalPosition;
    }

    void OnEnable()
    {
        sortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();
        if (sortingLayerNames.Length > 0)
        {
            selectedLayerIndex = 0;
            targetSortingLayer = sortingLayerNames[0];
        }
    }

    void OnGUI()
    {
        if (parentObject == null)
        {
            EditorGUILayout.HelpBox("请先选中一个父物体", MessageType.Warning);
            return;
        }

        GUILayout.Label($"根据Z深度设置OrderInLayer: {parentObject.name}", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 说明
        EditorGUILayout.HelpBox(
            "功能说明：\n" +
            "1. 根据当前Z深度自动计算并设置OrderInLayer\n" +
            "2. 可选择将所有子物体的Z轴归零（保持渲染层级不变）",
            MessageType.Info);

        GUILayout.Space(5);

        // 选项
        recursiveSearch = EditorGUILayout.Toggle("递归搜索所有子物体", recursiveSearch);
        useWorldPosition = EditorGUILayout.Toggle("使用世界坐标Z（用于计算Order）", useWorldPosition);
        includeInactive = EditorGUILayout.Toggle("包含未激活的物体", includeInactive);
        reverseOrder = EditorGUILayout.Toggle("反转顺序（Z越小Order越小）", reverseOrder);

        GUILayout.Space(5);

        // Sorting Layer选择
        int newLayerIndex = EditorGUILayout.Popup("目标Sorting Layer", selectedLayerIndex, sortingLayerNames);
        if (newLayerIndex != selectedLayerIndex)
        {
            selectedLayerIndex = newLayerIndex;
            targetSortingLayer = sortingLayerNames[selectedLayerIndex];
        }

        GUILayout.Space(5);

        // Order设置
        orderStart = EditorGUILayout.IntField("起始Order（最大Z）", orderStart);
        orderStep = EditorGUILayout.IntField("Order步长", orderStep);

        GUILayout.Space(5);

        // Z轴归零选项
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Z轴归零设置:", EditorStyles.boldLabel);
        resetZToZero = EditorGUILayout.Toggle("将所有子物体Z轴归零", resetZToZero);

        if (resetZToZero)
        {
            EditorGUI.indentLevel++;
            resetLocalPosition = EditorGUILayout.Toggle("重置局部坐标（而非世界坐标）", resetLocalPosition);
            EditorGUILayout.HelpBox(
                "勾选后，会将所有子物体的Z轴设置为0，\n" +
                "同时根据原有的Z深度设置OrderInLayer，\n" +
                "这样归零后渲染层级保持不变。",
                MessageType.None);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        // 预览规则示例
        EditorGUILayout.LabelField("规则示例:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"  最大Z（最远） → Order = {orderStart}");
        EditorGUILayout.LabelField($"  中间Z → Order = {orderStart + orderStep}");
        EditorGUILayout.LabelField($"  最小Z（最近） → Order = {orderStart + orderStep} × N");

        GUILayout.Space(10);

        // 扫描按钮
        if (GUILayout.Button("扫描并预览", GUILayout.Height(25)))
        {
            ScanAndPreview();
        }

        GUILayout.Space(10);

        // 显示预览
        if (rendererList != null && rendererList.Count > 0)
        {
            EditorGUILayout.LabelField($"找到 {rendererList.Count} 个Renderer:", EditorStyles.boldLabel);

            // 统计信息
            float minZ = rendererList.Min(r => r.zDepth);
            float maxZ = rendererList.Max(r => r.zDepth);
            EditorGUILayout.LabelField($"Z范围: {minZ:F3} ~ {maxZ:F3}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Order范围: {rendererList.Min(r => r.newOrder)} ~ {rendererList.Max(r => r.newOrder)}", EditorStyles.miniLabel);

            if (resetZToZero)
            {
                EditorGUILayout.LabelField($"⚠ 将同时重置所有子物体Z轴为0", EditorStyles.miniLabel);
            }

            GUILayout.Space(5);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));

            // 表头
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("物体", GUILayout.Width(150));
            EditorGUILayout.LabelField("当前Z", GUILayout.Width(60));
            EditorGUILayout.LabelField("归零后Z", GUILayout.Width(60));
            EditorGUILayout.LabelField("原Order", GUILayout.Width(60));
            EditorGUILayout.LabelField("新Order", GUILayout.Width(60));
            EditorGUILayout.LabelField("层级", GUILayout.Width(60));
            EditorGUILayout.LabelField("路径", GUILayout.MinWidth(150));
            EditorGUILayout.EndHorizontal();

            // 分隔线
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            foreach (var info in rendererList.OrderBy(r => r.zDepth))
            {
                EditorGUILayout.BeginHorizontal();

                // 物体名称
                Color originalColor = GUI.color;
                if (info.newOrder != info.originalOrder)
                    GUI.color = Color.green;

                EditorGUILayout.LabelField(info.renderer.name, GUILayout.Width(150));
                GUI.color = originalColor;

                // 当前Z
                EditorGUILayout.LabelField($"{info.zDepth:F3}", GUILayout.Width(60));

                // 归零后Z
                if (resetZToZero)
                    EditorGUILayout.LabelField("0", GUILayout.Width(60));
                else
                    EditorGUILayout.LabelField("-", GUILayout.Width(60));

                // 原Order
                EditorGUILayout.LabelField($"{info.originalOrder}", GUILayout.Width(60));

                // 新Order高亮
                if (info.newOrder != info.originalOrder)
                    GUI.color = Color.green;
                EditorGUILayout.LabelField($"{info.newOrder}", GUILayout.Width(60));
                GUI.color = originalColor;

                // 显示层级
                string layerTag = info.zDepth == minZ ? "最近↑" : (info.zDepth == maxZ ? "最远↓" : "");
                EditorGUILayout.LabelField(layerTag, GUILayout.Width(60));

                EditorGUILayout.LabelField(info.path, EditorStyles.miniLabel, GUILayout.MinWidth(150));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            // 快捷设置按钮
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("全部设置为同一SortingLayer", GUILayout.Height(30)))
            {
                SetAllToSameLayer();
            }

            if (GUILayout.Button("根据Z深度设置Order", GUILayout.Height(30)))
            {
                ApplyOrderByDepth();
                Close();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            string buttonText = resetZToZero ? "同时设置Layer、Order并归零Z轴" : "同时设置Layer和Order";
            if (GUILayout.Button(buttonText, GUILayout.Height(35)))
            {
                SetLayerAndOrder();
                Close();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("点击\"扫描并预览\"查看所有Renderer", MessageType.Info);
        }
    }

    void ScanAndPreview()
    {
        if (parentObject == null) return;

        rendererList = new List<RendererInfo>();

        // 获取所有Renderer
        IEnumerable<Renderer> renderers;
        if (recursiveSearch)
        {
            renderers = parentObject.GetComponentsInChildren<Renderer>(includeInactive);
        }
        else
        {
            renderers = parentObject.transform.Cast<Transform>()
                .Select(t => t.GetComponent<Renderer>())
                .Where(r => r != null);
        }

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;

            RendererInfo info = new RendererInfo();
            info.renderer = renderer;

            // 获取Z深度
            if (useWorldPosition)
                info.zDepth = renderer.transform.position.z;
            else
                info.zDepth = renderer.transform.localPosition.z;

            info.originalOrder = renderer.sortingOrder;
            info.path = GetRelativePath(parentObject.transform, renderer.transform);
            info.originalPosition = renderer.transform.position;

            rendererList.Add(info);
        }

        // 计算新Order
        CalculateNewOrders();

        Debug.Log($"扫描完成，找到 {rendererList.Count} 个Renderer");
    }

    void CalculateNewOrders()
    {
        if (rendererList == null || rendererList.Count == 0) return;

        // 按Z深度排序（Z从小到大）
        var sortedList = rendererList.OrderBy(r => r.zDepth).ToList();

        // 如果需要反转（Z越小Order越小）
        if (reverseOrder)
        {
            sortedList = sortedList.OrderByDescending(r => r.zDepth).ToList();
        }

        // 计算基础Order值
        int currentOrder = orderStart;
        float lastZ = sortedList[0].zDepth;

        for (int i = 0; i < sortedList.Count; i++)
        {
            var info = sortedList[i];

            // 如果Z值与上一个不同，增加步长
            if (Mathf.Abs(info.zDepth - lastZ) > 0.0001f && i > 0)
            {
                currentOrder += orderStep;
                lastZ = info.zDepth;
            }

            info.newOrder = currentOrder;
        }

        // 如果不是反转模式，但我们需要Z越小Order越大，所以反转Order值
        if (!reverseOrder)
        {
            int maxOrder = sortedList.Max(r => r.newOrder);
            foreach (var info in rendererList)
            {
                info.newOrder = maxOrder - info.newOrder + orderStart;
            }
        }
    }

    void SetAllToSameLayer()
    {
        if (rendererList == null || rendererList.Count == 0)
        {
            ScanAndPreview();
        }

        List<Renderer> allRenderers = rendererList.Select(r => r.renderer).ToList();
        Undo.RecordObjects(allRenderers.ToArray(), "Set Sorting Layer");

        foreach (var info in rendererList)
        {
            info.renderer.sortingLayerName = targetSortingLayer;
            EditorUtility.SetDirty(info.renderer);
        }

        Debug.Log($"已将 {rendererList.Count} 个Renderer的SortingLayer设置为 {targetSortingLayer}");

        // 刷新预览
        ScanAndPreview();
    }

    void ApplyOrderByDepth()
    {
        if (rendererList == null || rendererList.Count == 0)
        {
            ScanAndPreview();
        }

        // 收集所有需要记录的物体（包括Transform）
        List<Object> objectsToRecord = new List<Object>();
        objectsToRecord.AddRange(rendererList.Select(r => r.renderer));

        if (resetZToZero)
        {
            objectsToRecord.AddRange(rendererList.Select(r => r.renderer.transform));
        }

        Undo.RecordObjects(objectsToRecord.ToArray(), "Set Order By Depth");

        // 设置Order
        foreach (var info in rendererList)
        {
            info.renderer.sortingOrder = info.newOrder;
            EditorUtility.SetDirty(info.renderer);
        }

        // 如果需要重置Z轴
        if (resetZToZero)
        {
            foreach (var info in rendererList)
            {
                Transform transform = info.renderer.transform;
                if (resetLocalPosition)
                {
                    // 重置局部坐标Z
                    Vector3 localPos = transform.localPosition;
                    localPos.z = 0;
                    transform.localPosition = localPos;
                }
                else
                {
                    // 重置世界坐标Z
                    Vector3 worldPos = transform.position;
                    worldPos.z = 0;
                    transform.position = worldPos;
                }
                EditorUtility.SetDirty(transform);
            }
            Debug.Log($"已将 {rendererList.Count} 个物体的Z轴归零");
        }

        Debug.Log($"已根据Z深度设置 {rendererList.Count} 个Renderer的Order\n" +
                 $"规则：Z越小（越近）→ Order越大（上层）");
    }

    void SetLayerAndOrder()
    {
        if (rendererList == null || rendererList.Count == 0)
        {
            ScanAndPreview();
        }

        // 收集所有需要记录的物体
        List<Object> objectsToRecord = new List<Object>();
        objectsToRecord.AddRange(rendererList.Select(r => r.renderer));

        if (resetZToZero)
        {
            objectsToRecord.AddRange(rendererList.Select(r => r.renderer.transform));
        }

        Undo.RecordObjects(objectsToRecord.ToArray(), "Set Layer And Order");

        // 设置Layer和Order
        foreach (var info in rendererList)
        {
            info.renderer.sortingLayerName = targetSortingLayer;
            info.renderer.sortingOrder = info.newOrder;
            EditorUtility.SetDirty(info.renderer);
        }

        // 如果需要重置Z轴
        if (resetZToZero)
        {
            foreach (var info in rendererList)
            {
                Transform transform = info.renderer.transform;
                if (resetLocalPosition)
                {
                    Vector3 localPos = transform.localPosition;
                    localPos.z = 0;
                    transform.localPosition = localPos;
                }
                else
                {
                    Vector3 worldPos = transform.position;
                    worldPos.z = 0;
                    transform.position = worldPos;
                }
                EditorUtility.SetDirty(transform);
            }
            Debug.Log($"已将 {rendererList.Count} 个物体的Z轴归零");
        }

        Debug.Log($"已完成！设置 {rendererList.Count} 个Renderer:\n" +
                 $"SortingLayer: {targetSortingLayer}\n" +
                 $"Order范围: {rendererList.Min(r => r.newOrder)} ~ {rendererList.Max(r => r.newOrder)}\n" +
                 $"规则：Z越小（越近）→ Order越大（上层）" +
                 (resetZToZero ? $"\nZ轴已全部归零" : ""));
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
}