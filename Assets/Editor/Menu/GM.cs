using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GM:EditorWindow
{
    private static int sunNum = 0;
    private IntegerField levelCountField;
    private IntegerField sunCountField;
    private IntegerField coinCountField;
    
    [MenuItem("Myh/GM")]
    public static void ShowGm()
    {
        // EditorWindow window = GetWindow<GM>("PvZ GM");
        // VisualElement visualElement = new VisualElement();
        // Button sureBtn = new Button{text = "确定"};
        // sureBtn.clicked += SureBtnClick;
        // visualElement.Add(sureBtn);
        // window.rootVisualElement.Add(visualElement);
        // window.rootVisualElement.Add(Test());
        // window.rootVisualElement.Add(new TextField(){value = sunNum.ToString()});
        var wnd = GetWindow<GM>();
        wnd.titleContent = new GUIContent("PvZ GM");
    }
    
    public void CreateGUI()
    {
        // 根容器
        var root = rootVisualElement;

        // 样式
        root.style.paddingLeft = 10;
        root.style.paddingRight = 10;
        root.style.paddingTop = 10;

        // 关卡数量
        levelCountField = new IntegerField("关卡数量") { value = 0 };
        root.Add(levelCountField);

        // 阳光数量
        sunCountField = new IntegerField("阳光数量") { value = 0 };
        root.Add(sunCountField);

        // 金币数量
        coinCountField = new IntegerField("金币数量") { value = 0 };
        root.Add(coinCountField);

        // 保存按钮
        var saveButton = new Button(() =>
        {
            Debug.Log($"关卡数量: {levelCountField.value}");
            Debug.Log($"阳光数量: {sunCountField.value}");
            Debug.Log($"金币数量: {coinCountField.value}");
            SunManager.GetInstance().GetSun(sunCountField.value);
            // 这里可以写保存逻辑，比如存到 ScriptableObject 或 JSON
        })
        {
            text = "设置"
        };

        saveButton.style.marginTop = 10;
        root.Add(saveButton);
    }

    private static VisualElement Test()
    {
        Label label = new Label("设置阳光数量");
        // label.Add(new Button{text = "确定"});
        
        return label;
    }

    private static void SureBtnClick()
    {
        Debug.Log(sunNum);
    }
    
    private void OnGUI()
    {


    }
}