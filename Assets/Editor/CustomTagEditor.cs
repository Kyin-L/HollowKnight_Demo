#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(CustomTag))]
public class CustomTagEditor : Editor
{
    private string[] recoilNames;
    private SerializedProperty recoilProp;

    private void OnEnable()
    {
        recoilNames = System.Enum.GetNames(typeof(RecoilTag));
        recoilProp = serializedObject.FindProperty("recoilTag");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ===== 后坐力标签 =====
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("后坐力方向（可多选）", EditorStyles.boldLabel);

        int displayValue = recoilProp.intValue;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("方向", GUILayout.Width(40));

        string buttonText = GetDisplayText(displayValue);
        if (GUILayout.Button(buttonText, EditorStyles.popup))
        {
            GenericMenu menu = new GenericMenu();

            // 为每个标签添加菜单项
            for (int i = 0; i < recoilNames.Length; i++)
            {
                int flag = 1 << i;
                string name = recoilNames[i];
                bool isSelected = (displayValue & flag) != 0;
                int capturedFlag = flag;

                menu.AddItem(new GUIContent(name), isSelected, () =>
                {
                    int newValue = recoilProp.intValue;
                    if ((newValue & capturedFlag) != 0)
                        newValue &= ~capturedFlag;
                    else
                        newValue |= capturedFlag;
                    recoilProp.intValue = newValue;
                    serializedObject.ApplyModifiedProperties();
                });
            }

            menu.AddSeparator("");

            // 添加 None 选项
            menu.AddItem(new GUIContent("None"), displayValue == 0, () =>
            {
                recoilProp.intValue = 0;
                serializedObject.ApplyModifiedProperties();
            });

            // 添加 All 选项
            int allValue = 0;
            for (int i = 0; i < recoilNames.Length; i++)
                allValue |= (1 << i);
            menu.AddItem(new GUIContent("All"), displayValue == allValue, () =>
            {
                recoilProp.intValue = allValue;
                serializedObject.ApplyModifiedProperties();
            });

            menu.ShowAsContext();
        }

        EditorGUILayout.EndHorizontal();

        if (displayValue != 0)
        {
            EditorGUILayout.HelpBox($"当前：{GetDisplayText(displayValue)}", MessageType.Info);
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private string GetDisplayText(int value)
    {
        if (value == 0) return "None";

        List<string> selected = new List<string>();
        for (int i = 0; i < recoilNames.Length; i++)
        {
            int flag = 1 << i;
            if ((value & flag) != 0)
            {
                selected.Add(recoilNames[i]);
            }
        }
        return string.Join(", ", selected);
    }
}
#endif