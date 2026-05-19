using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class SortChildrenByZ : MonoBehaviour
{
    [MenuItem("GameObject/按Z排序子物体", false, 0)]
    static void QuickSortAscending(MenuCommand command)
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null) return;

        Undo.RegisterFullObjectHierarchyUndo(obj, "Quick Sort By Z");

        // 获取并排序子物体
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < obj.transform.childCount; i++)
            children.Add(obj.transform.GetChild(i));

        children = children.OrderBy(t => t.localPosition.z).ToList();

        // 重排Hierarchy
        for (int i = 0; i < children.Count; i++)
            children[i].SetSiblingIndex(i);

        // 设置OrderInLayer
//         Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
//         Dictionary<Transform, Renderer> dict = renderers.ToDictionary(r => r.transform, r => r);
// 
//         for (int i = 0; i < children.Count; i++)
//         {
//             if (dict.ContainsKey(children[i]))
//             {
//                 dict[children[i]].sortingOrder = i;
//                 EditorUtility.SetDirty(dict[children[i]]);
//             }
//         }
// 
//         Debug.Log($"✅ 已排序并设置Order: {obj.name}");
    }

    [MenuItem("GameObject/按Z排序子物体并设置Order（升序）", true)]
    static bool Validate()
    {
        return Selection.activeGameObject != null;
    }
}
