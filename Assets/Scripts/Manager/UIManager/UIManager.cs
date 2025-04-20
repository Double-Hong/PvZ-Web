using UnityEngine;

public class UIManager
{
    private static IUiPathProvider uiPathProvider;

    private static Transform root;

    public static void Init(IUiPathProvider provider,Transform rootTransform)
    {
        SetUiPathProvider(provider);
        root = rootTransform;
    }
    
    private static void SetUiPathProvider(IUiPathProvider provider)
    {
        uiPathProvider = provider;
    }
    
    public static void Show(string uiName)
    {
        if (uiPathProvider == null)
        {
            Debug.LogError("未初始化UI路径提供者,请调用UIManager.Init()");
            return;
        }

        string path = uiPathProvider.GetPath(uiName);
        
        GameObject ui = Resources.Load<GameObject>(path);
        
        ui = (GameObject)Object.Instantiate(ui, root);
        ui.GetComponent<BaseView>().Show();
        
    }
}