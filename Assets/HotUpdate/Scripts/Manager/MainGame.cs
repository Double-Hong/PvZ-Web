using System;
using System.Reflection;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    // Start is called before the first frame update
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
