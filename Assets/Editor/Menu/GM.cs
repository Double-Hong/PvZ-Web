using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GM:EditorWindow
{
    private string sunNum;
    
    [MenuItem("Myh/GM")]
    public static void ShowGm()
    {
        EditorWindow window = GetWindow<GM>("PvZ GM");
        VisualElement visualElement = new VisualElement();
        visualElement.Add(new Button{text = "确定"});
        window.rootVisualElement.Add(visualElement);
    }

    private void OnGUI()
    {


    }
}