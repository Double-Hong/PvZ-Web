using System;
using System.Reflection;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    //TODO 放到热更层初始化
    void Start()
    {
        InitModule();
        InitMainModel();
    }

    private void InitModule()
    {
        var ass = Assembly.GetExecutingAssembly();
        Type[] types = ass.GetTypes();
        foreach (var type in types)
        {
            if (type.IsSubclassOf(typeof(BaseController)))
            {
                ControllerTool.InitController(type);
            }
        }
    }

    private void InitMainModel()
    {
        PlantModel.Inst.Init();
    }
}
