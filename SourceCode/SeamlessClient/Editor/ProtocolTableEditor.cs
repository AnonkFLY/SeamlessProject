using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProtocolValue))]
public class ProtocolTableEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 在父属性上使用 BeginProperty/EndProperty 意味着
        // 预制件重写逻辑作用于整个属性。
        EditorGUI.BeginProperty(position, label, property);

        //绘制标签
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // 不要让子字段缩进
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        position.x -= 150;
        // 计算矩形
        var idRect = new Rect(position.x, position.y, 20, position.height);
        var boundRect = new Rect(position.x + 35, position.y, 60, position.height);
        var brieflyRect = new Rect(position.x + 100, position.y, 80, position.height);
        var describeRect = new Rect(position.x + 190, position.y, position.width - 45, position.height);

        // 绘制字段 - 将 GUIContent.none 传入每个字段，从而可以不使用标签来绘制字段
        EditorGUI.PropertyField(idRect, property.FindPropertyRelative("packetID"), GUIContent.none);
        EditorGUI.PropertyField(boundRect, property.FindPropertyRelative("boundTo"), GUIContent.none);
        EditorGUI.PropertyField(brieflyRect, property.FindPropertyRelative("briefly"), GUIContent.none);
        EditorGUI.PropertyField(describeRect, property.FindPropertyRelative("describe"), GUIContent.none);

        // 将缩进恢复原样
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}