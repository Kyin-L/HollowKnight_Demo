using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(BreakableConfig))]
public class BreakableConfigEditor : Editor
{
    private SerializedProperty hp;
    private SerializedProperty feedbacks;
    private SerializedProperty hitEffects;
    private static Dictionary<string, object> clipboard = new Dictionary<string, object>();

    private void OnEnable()
    {
        hp = serializedObject.FindProperty("hp");
        feedbacks = serializedObject.FindProperty("feedbacks");
        hitEffects = serializedObject.FindProperty("hitEffects");

        if (feedbacks != null && (feedbacks.arraySize == 0 || feedbacks.arraySize != hp.intValue))
        {
            AdjustFeedbacksLength();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 基础属性
        EditorGUILayout.LabelField("基础属性", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUI.BeginChangeCheck();

        int newHp = EditorGUILayout.IntField(new GUIContent("Hp"), hp.intValue);
        if (newHp < 1) newHp = 1;
        hp.intValue = newHp;

        if (EditorGUI.EndChangeCheck())
        {
            AdjustFeedbacksLength();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        // 使用 Unity 默认方式绘制 hitEffects，但添加右键菜单功能
        DrawDefaultHitEffects();

        // 显示反馈列表
        if (feedbacks != null && feedbacks.arraySize > 0)
        {
            if (hp.intValue > 1)
            {
                EditorGUILayout.LabelField("受击反馈", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.HelpBox(
                    $"需要配置 {hp.intValue - 1} 个受击反馈\n提示：右键点击反馈可以复制/粘贴",
                    MessageType.Info
                );

                for (int i = 0; i < feedbacks.arraySize - 1; i++)
                {
                    var hitFeedback = feedbacks.GetArrayElementAtIndex(i);
                    if (hitFeedback != null)
                    {
                        DrawFeedbackWithContextMenu(hitFeedback, $"第 {i + 1} 次受击", i);
                    }
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("破坏反馈", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var breakFeedback = feedbacks.GetArrayElementAtIndex(feedbacks.arraySize - 1);
            if (breakFeedback != null)
            {
                DrawFeedbackWithContextMenu(breakFeedback, "破坏反馈配置", -1);
            }
            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUILayout.HelpBox("反馈列表为空，请修改 hp 值以自动生成反馈条目", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultHitEffects()
    {
        // 使用 Unity 默认的 PropertyField 绘制，完全保持原生样式
        EditorGUILayout.PropertyField(hitEffects, new GUIContent("受击特效"), true);

        // 获取绘制区域，添加右键菜单
        Rect lastRect = GUILayoutUtility.GetLastRect();

        // 只在头部区域添加右键菜单
        Rect headerRect = new Rect(lastRect.x, lastRect.y, lastRect.width, EditorGUIUtility.singleLineHeight);

        Event currentEvent = Event.current;
        if (headerRect.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy All Hit Effects"), false, () => CopyWholeHitEffectsList());

            if (clipboard.ContainsKey("CopiedHitEffectsList"))
            {
                menu.AddItem(new GUIContent("Paste All Hit Effects (Replace)"), false, () => PasteWholeHitEffectsList());
                menu.AddItem(new GUIContent("Paste All Hit Effects (Append)"), false, () => AppendWholeHitEffectsList());
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste All Hit Effects (No Data)"));
            }

            menu.AddSeparator("");

            if (hitEffects.arraySize > 0)
            {
                menu.AddItem(new GUIContent("Clear All Hit Effects"), false, () =>
                {
                    if (EditorUtility.DisplayDialog("确认清空", "确定要清空所有受击特效吗？", "确定", "取消"))
                    {
                        hitEffects.ClearArray();
                        serializedObject.ApplyModifiedProperties();
                    }
                });
            }

            menu.ShowAsContext();
            currentEvent.Use();
        }
    }

    private void CopyWholeHitEffectsList()
    {
        var copiedList = new List<HitEffectData>();

        for (int i = 0; i < hitEffects.arraySize; i++)
        {
            var element = hitEffects.GetArrayElementAtIndex(i);
            var hitEffectPrefab = element.FindPropertyRelative("hitEffectPrefab");
            var needLookAt = element.FindPropertyRelative("needLookAt");

            var data = new HitEffectData();
            data.hitEffectPrefab = hitEffectPrefab.objectReferenceValue as GameObject;
            data.needLookAt = needLookAt.boolValue;
            copiedList.Add(data);
        }

        clipboard["CopiedHitEffectsList"] = copiedList;
        Debug.Log($"已复制整个受击特效列表，共 {copiedList.Count} 项");
    }

    private void PasteWholeHitEffectsList()
    {
        if (!clipboard.ContainsKey("CopiedHitEffectsList")) return;

        var copiedList = clipboard["CopiedHitEffectsList"] as List<HitEffectData>;
        if (copiedList == null) return;

        hitEffects.ClearArray();

        foreach (var data in copiedList)
        {
            hitEffects.InsertArrayElementAtIndex(hitEffects.arraySize);
            var newElement = hitEffects.GetArrayElementAtIndex(hitEffects.arraySize - 1);
            var hitEffectPrefab = newElement.FindPropertyRelative("hitEffectPrefab");
            var needLookAt = newElement.FindPropertyRelative("needLookAt");

            hitEffectPrefab.objectReferenceValue = data.hitEffectPrefab;
            needLookAt.boolValue = data.needLookAt;
        }

        serializedObject.ApplyModifiedProperties();
        Debug.Log($"已粘贴受击特效列表（替换），共 {copiedList.Count} 项");
    }

    private void AppendWholeHitEffectsList()
    {
        if (!clipboard.ContainsKey("CopiedHitEffectsList")) return;

        var copiedList = clipboard["CopiedHitEffectsList"] as List<HitEffectData>;
        if (copiedList == null) return;

        foreach (var data in copiedList)
        {
            hitEffects.InsertArrayElementAtIndex(hitEffects.arraySize);
            var newElement = hitEffects.GetArrayElementAtIndex(hitEffects.arraySize - 1);
            var hitEffectPrefab = newElement.FindPropertyRelative("hitEffectPrefab");
            var needLookAt = newElement.FindPropertyRelative("needLookAt");

            hitEffectPrefab.objectReferenceValue = data.hitEffectPrefab;
            needLookAt.boolValue = data.needLookAt;
        }

        serializedObject.ApplyModifiedProperties();
        Debug.Log($"已追加受击特效列表，共 {copiedList.Count} 项");
    }

    private void DrawFeedbackWithContextMenu(SerializedProperty feedback, string label, int index)
    {
        string propertyPath = feedback.propertyPath;

        var sound = feedback.FindPropertyRelative("sound");
        var animation = feedback.FindPropertyRelative("animation");
        var effectPrefabs = feedback.FindPropertyRelative("effectPrefabs");
        var dropItemsPrefabs = feedback.FindPropertyRelative("dropItemsPrefabs");

        EditorGUILayout.BeginVertical("box");

        bool isExpanded = SessionState.GetBool(propertyPath, true);
        EditorGUILayout.BeginHorizontal();

        Rect foldoutRect = EditorGUILayout.GetControlRect();

        isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, label, true);
        SessionState.SetBool(propertyPath, isExpanded);

        Event currentEvent = Event.current;
        if (foldoutRect.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy"), false, () => CopyFeedback(feedback));

            bool hasClipboardData = clipboard.ContainsKey("CopiedFeedback");
            if (hasClipboardData)
            {
                menu.AddItem(new GUIContent("Paste"), false, () => PasteFeedback(feedback));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste (无复制数据)"));
            }

            menu.AddSeparator("");

            if (index == -1)
            {
                menu.AddItem(new GUIContent("Reset to Default (Break)"), false, () => ResetFeedbackToDefault(feedback, true));
            }
            else
            {
                menu.AddItem(new GUIContent("Reset to Default (Hit)"), false, () => ResetFeedbackToDefault(feedback, false));
            }

            menu.ShowAsContext();
            currentEvent.Use();
        }

        EditorGUILayout.EndHorizontal();

        if (isExpanded)
        {
            EditorGUI.indentLevel++;

            if (sound != null)
                EditorGUILayout.PropertyField(sound);

            if (animation != null)
                EditorGUILayout.PropertyField(animation);

            EditorGUILayout.Space();

            if (effectPrefabs != null)
                DrawArrayWithDefaultValues(effectPrefabs, "Effect Prefabs", ResetEffectPrefabToDefault);

            if (dropItemsPrefabs != null)
                DrawArrayWithDefaultValues(dropItemsPrefabs, "Drop Items Prefabs", ResetDropsItemToDefault);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private void CopyFeedback(SerializedProperty feedback)
    {
        var tempFeedback = new BreakableConfig.Feedback();

        var soundProp = feedback.FindPropertyRelative("sound");
        if (soundProp != null)
            tempFeedback.sound = soundProp.objectReferenceValue as AudioClip;

        var animationProp = feedback.FindPropertyRelative("animation");
        if (animationProp != null)
            tempFeedback.animation = animationProp.stringValue;

        var effectPrefabsProp = feedback.FindPropertyRelative("effectPrefabs");
        if (effectPrefabsProp != null && effectPrefabsProp.arraySize > 0)
        {
            tempFeedback.effectPrefabs = new GameObject[effectPrefabsProp.arraySize];
            for (int i = 0; i < effectPrefabsProp.arraySize; i++)
            {
                var element = effectPrefabsProp.GetArrayElementAtIndex(i);
                tempFeedback.effectPrefabs[i] = element.objectReferenceValue as GameObject;
            }
        }
        else
        {
            tempFeedback.effectPrefabs = new GameObject[0];
        }

        var dropItemsProp = feedback.FindPropertyRelative("dropItemsPrefabs");
        if (dropItemsProp != null && dropItemsProp.arraySize > 0)
        {
            tempFeedback.dropItemsPrefabs = new BreakableConfig.DropsItem[dropItemsProp.arraySize];
            for (int i = 0; i < dropItemsProp.arraySize; i++)
            {
                var element = dropItemsProp.GetArrayElementAtIndex(i);
                var newItem = new BreakableConfig.DropsItem();

                var prefabProp = element.FindPropertyRelative("prefab");
                if (prefabProp != null)
                    newItem.prefab = prefabProp.objectReferenceValue as GameObject;

                var countRangeProp = element.FindPropertyRelative("countRange");
                if (countRangeProp != null)
                    newItem.countRange = countRangeProp.vector2IntValue;

                var forceRangeProp = element.FindPropertyRelative("forceRange");
                if (forceRangeProp != null)
                    newItem.forceRange = forceRangeProp.vector2Value;

                tempFeedback.dropItemsPrefabs[i] = newItem;
            }
        }
        else
        {
            tempFeedback.dropItemsPrefabs = new BreakableConfig.DropsItem[0];
        }

        clipboard["CopiedFeedback"] = tempFeedback;
    }

    private void PasteFeedback(SerializedProperty targetFeedback)
    {
        if (!clipboard.ContainsKey("CopiedFeedback"))
        {
            Debug.LogWarning("剪贴板中没有复制的反馈数据");
            return;
        }

        var copiedData = clipboard["CopiedFeedback"] as BreakableConfig.Feedback;
        if (copiedData == null) return;

        var soundProp = targetFeedback.FindPropertyRelative("sound");
        if (soundProp != null)
            soundProp.objectReferenceValue = copiedData.sound;

        var animationProp = targetFeedback.FindPropertyRelative("animation");
        if (animationProp != null)
            animationProp.stringValue = copiedData.animation;

        var effectPrefabsProp = targetFeedback.FindPropertyRelative("effectPrefabs");
        if (effectPrefabsProp != null)
        {
            effectPrefabsProp.ClearArray();
            if (copiedData.effectPrefabs != null)
            {
                for (int i = 0; i < copiedData.effectPrefabs.Length; i++)
                {
                    effectPrefabsProp.InsertArrayElementAtIndex(i);
                    var element = effectPrefabsProp.GetArrayElementAtIndex(i);
                    element.objectReferenceValue = copiedData.effectPrefabs[i];
                }
            }
        }

        var dropItemsProp = targetFeedback.FindPropertyRelative("dropItemsPrefabs");
        if (dropItemsProp != null)
        {
            dropItemsProp.ClearArray();
            if (copiedData.dropItemsPrefabs != null)
            {
                for (int i = 0; i < copiedData.dropItemsPrefabs.Length; i++)
                {
                    dropItemsProp.InsertArrayElementAtIndex(i);
                    var element = dropItemsProp.GetArrayElementAtIndex(i);
                    var sourceItem = copiedData.dropItemsPrefabs[i];

                    var prefabProp = element.FindPropertyRelative("prefab");
                    if (prefabProp != null)
                        prefabProp.objectReferenceValue = sourceItem.prefab;

                    var countRangeProp = element.FindPropertyRelative("countRange");
                    if (countRangeProp != null)
                        countRangeProp.vector2IntValue = sourceItem.countRange;

                    var forceRangeProp = element.FindPropertyRelative("forceRange");
                    if (forceRangeProp != null)
                        forceRangeProp.vector2Value = sourceItem.forceRange;
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawArrayWithDefaultValues(SerializedProperty arrayProperty, string label, System.Action<SerializedProperty> resetToDefault)
    {
        int originalSize = arrayProperty.arraySize;

        EditorGUILayout.PropertyField(arrayProperty, new GUIContent(label), true);

        if (arrayProperty.arraySize > originalSize)
        {
            for (int i = originalSize; i < arrayProperty.arraySize; i++)
            {
                var newElement = arrayProperty.GetArrayElementAtIndex(i);
                resetToDefault?.Invoke(newElement);
            }
        }
    }

    private void ResetEffectPrefabToDefault(SerializedProperty element)
    {
        if (element == null) return;
        element.objectReferenceValue = null;
    }

    private void ResetDropsItemToDefault(SerializedProperty element)
    {
        if (element == null) return;

        var defaultItem = BreakableConfig.DropsItem.GetDefault();

        var prefabProperty = element.FindPropertyRelative("prefab");
        if (prefabProperty != null)
            prefabProperty.objectReferenceValue = defaultItem.prefab;

        var countRangeProperty = element.FindPropertyRelative("countRange");
        if (countRangeProperty != null)
            countRangeProperty.vector2IntValue = defaultItem.countRange;

        var forceRangeProperty = element.FindPropertyRelative("forceRange");
        if (forceRangeProperty != null)
            forceRangeProperty.vector2Value = defaultItem.forceRange;
    }

    private void AdjustFeedbacksLength()
    {
        if (feedbacks == null || hp == null)
            return;

        int targetCount = Mathf.Max(1, hp.intValue);
        int oldSize = feedbacks.arraySize;

        if (oldSize == targetCount)
            return;

        if (oldSize < targetCount)
        {
            int insertCount = targetCount - oldSize;
            for (int i = 0; i < insertCount; i++)
            {
                int insertIndex = Mathf.Max(0, feedbacks.arraySize - 1);
                feedbacks.InsertArrayElementAtIndex(insertIndex);
                var newFeedback = feedbacks.GetArrayElementAtIndex(insertIndex);
                bool isBreak = (feedbacks.arraySize - 1 == insertIndex);
                ResetFeedbackToDefault(newFeedback, isBreak);
            }
        }
        else
        {
            int deleteCount = oldSize - targetCount;
            for (int i = 0; i < deleteCount; i++)
            {
                if (feedbacks.arraySize > 1)
                {
                    feedbacks.DeleteArrayElementAtIndex(0);
                }
            }
        }

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    private void ResetFeedbackToDefault(SerializedProperty feedback, bool isBreak)
    {
        if (feedback == null) return;

        var defaultFeedback = BreakableConfig.Feedback.GetDefault(isBreak);

        var soundProperty = feedback.FindPropertyRelative("sound");
        if (soundProperty != null)
            soundProperty.objectReferenceValue = defaultFeedback.sound;

        var animationProperty = feedback.FindPropertyRelative("animation");
        if (animationProperty != null)
            animationProperty.stringValue = defaultFeedback.animation;

        var effectPrefabsProperty = feedback.FindPropertyRelative("effectPrefabs");
        if (effectPrefabsProperty != null)
        {
            effectPrefabsProperty.ClearArray();
            if (defaultFeedback.effectPrefabs != null)
            {
                for (int i = 0; i < defaultFeedback.effectPrefabs.Length; i++)
                {
                    effectPrefabsProperty.InsertArrayElementAtIndex(i);
                    var element = effectPrefabsProperty.GetArrayElementAtIndex(i);
                    element.objectReferenceValue = defaultFeedback.effectPrefabs[i];
                }
            }
        }

        var dropItemsPrefabsProperty = feedback.FindPropertyRelative("dropItemsPrefabs");
        if (dropItemsPrefabsProperty != null)
        {
            dropItemsPrefabsProperty.ClearArray();
        }
    }

    [System.Serializable]
    private class HitEffectData
    {
        public GameObject hitEffectPrefab;
        public bool needLookAt;
    }
}