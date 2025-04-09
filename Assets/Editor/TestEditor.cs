using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    //This is the value of the Slider
    float m_Value;
    
    bool showPosition = true;
    string status = "Select a GameObject";

    public override void OnInspectorGUI()
    {
        // 获取目标对象（即 MyComponent 脚本的实例）
        Test myComponent = (Test)target;

        // 开始定义 Inspector GUI
        EditorGUILayout.LabelField("Custom Inspector", EditorStyles.boldLabel);

        // 绘制一个整数字段
        myComponent.intValue = EditorGUILayout.IntField("Integer Value", myComponent.intValue);

        // 绘制一个浮点数字段
        myComponent.floatValue = EditorGUILayout.FloatField("Float Value", myComponent.floatValue);

        // 绘制一个开关（Toggle）
        myComponent.isEnabled = EditorGUILayout.Toggle("Is Enabled", myComponent.isEnabled);

        // 绘制一个颜色选择器
        myComponent.colorValue = EditorGUILayout.ColorField("Color Value", myComponent.colorValue);

        // 添加一个按钮
        if (GUILayout.Button("Reset Values"))
        {
            myComponent.intValue = 0;
            myComponent.floatValue = 0.0f;
            myComponent.isEnabled = false;
            myComponent.colorValue = Color.white;
        }

        // 标记目标对象已更改，以确保 Unity 记录和保存更改
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        
        showPosition = EditorGUILayout.Foldout(showPosition, status);
        if (showPosition)
            if (Selection.activeTransform)
            {
                Selection.activeTransform.position =
                    EditorGUILayout.Vector3Field("Position", Selection.activeTransform.position);
                status = Selection.activeTransform.name;
            }

        if (!Selection.activeTransform)
        {
            status = "Select a GameObject";
            showPosition = false;
        }

        VisualElement element = new VisualElement();
    }
}
