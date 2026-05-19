using UnityEditor;
using UnityEngine;

public class MinValueAttribute : PropertyAttribute
{
    public float minValue;

    public MinValueAttribute(float minValue)
    {
        this.minValue = minValue;
    }
}

[CustomPropertyDrawer(typeof(MinValueAttribute))]
public class MinValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinValueAttribute minValueAttr = (MinValueAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            int value = EditorGUI.IntField(position, label, property.intValue);
            if (value < minValueAttr.minValue)
                value = (int)minValueAttr.minValue;
            property.intValue = value;
        }
        else if (property.propertyType == SerializedPropertyType.Float)
        {
            float value = EditorGUI.FloatField(position, label, property.floatValue);
            if (value < minValueAttr.minValue)
                value = minValueAttr.minValue;
            property.floatValue = value;
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use MinValue with int or float");
        }
    }
}