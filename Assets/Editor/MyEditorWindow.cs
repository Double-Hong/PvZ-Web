using System.Threading;
using GameData;
using UnityEditor;
using UnityEngine;

public class MyEditorWindow : EditorWindow
{
    private string myText = "";

    bool showBtn = true;

    public float secs = 5f;

    public ScriptableObject obj = null;


    [MenuItem("Myh/My Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<MyEditorWindow>("Custom GUIUtility Example");
    }

    private void OnGUI()
    {
        //设置控件名称，以便焦点管理
        GUI.SetNextControlName("MyTextField");
        myText = EditorGUILayout.TextField("Enter Text:", myText);

        if (GUILayout.Button("Focus on Text Field"))
        {
            // 使文本框获得焦点
            GUI.FocusControl("MyTextField");
        }

        // 获取键盘焦点的控件 ID
        if (GUI.GetNameOfFocusedControl() == "MyTextField")
        {
            EditorGUILayout.HelpBox("Text Field is focused", MessageType.Info);
        }

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        showBtn = EditorGUILayout.Toggle("Show Button", showBtn);

        secs = EditorGUILayout.Slider("Time to wait:", secs, 1.0f, 20.0f);
        if (GUILayout.Button("Display bar"))
        {
            var step = 0.1f;
            for (float t = 0; t < secs; t += step)
            {
                EditorUtility.DisplayProgressBar("Simple Progress Bar", "Doing some work...", t / secs);
                // Normally, some computation happens here.
                // This example uses Sleep.
                Thread.Sleep((int)(step * 1000.0f));
            }

            EditorUtility.ClearProgressBar();
        }
        
        obj = (ScriptableObject)EditorGUILayout.ObjectField("Find Dependency", obj, typeof(ScriptableObject));
    }

    [MenuItem("Myh/Enhanced Save")]
    static void Init()
    {
        Rect contextRect = new Rect(10, 10, 100, 100);
        EditorUtility.DisplayPopupMenu(contextRect, "Assets/", null);
        EditorUtility.ClearProgressBar();
    }
}