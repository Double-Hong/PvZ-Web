using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static IUiPathProvider uiPathProvider;

    private static Transform root;

    private static Dictionary<string, BaseView> viewDict = new();

    /// <summary>
    /// 初始化UI路径提供器
    /// </summary>
    /// <param name="provider">路径提供器</param>
    /// <param name="rootTransform">UI根路径</param>
    public static void Init(IUiPathProvider provider,Transform rootTransform)
    {
        SetUiPathProvider(provider);
        root = rootTransform;
    }
    
    private static void SetUiPathProvider(IUiPathProvider provider)
    {
        uiPathProvider = provider;
    }
    
    public static void Show(string uiName,params object[] args)
    {
        if (uiPathProvider == null)
        {
            Debug.LogError("未初始化UI路径提供器,请调用UIManager.Init()");
            return;
        }

        if (viewDict.TryGetValue(uiName,out BaseView value))
        {
            if (value != null)
            {
                value.Show(args);
                value.gameObject.SetActive(true);
                return;
            }

            viewDict.Remove(uiName);
        }

        string path = uiPathProvider.GetPath(uiName);
        
        GameObject ui = Resources.Load<GameObject>(path);
        
        ui = (GameObject)Object.Instantiate(ui, root);
        BaseView bv = ui.GetComponent<BaseView>();
        viewDict.Add(uiName,bv);
        bv.Show(args);
    }

    public static void Close(string uiName)
    {
        if (!viewDict.TryGetValue(uiName,out BaseView value))
        {
            Debug.LogError($"未找到{uiName}");
            return;
        }

        var view = value.GetComponent<BaseView>();
        if (view != null)
        {
            view.Close();
        }
        Object.Destroy(value.gameObject);
        viewDict.Remove(uiName);
    }

    public static void Hide(string uiName)
    {
        if (!viewDict.TryGetValue(uiName,out BaseView value))
        {
            Debug.LogError($"未找到{uiName}");
            return;
        }
        value.gameObject.SetActive(false);
    }
}