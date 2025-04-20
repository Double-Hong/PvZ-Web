using System;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTool
{
    private static Dictionary<Type, BaseController> controllerDict = new ();

    public static void InitController(Type type)
    {
        object instance = Activator.CreateInstance(type);
        BaseController controller = instance as BaseController;
        if (!controllerDict.ContainsKey(type))
        {
            controllerDict.Add(type,controller);
        }
    }

    public static T GetController<T>() where T : BaseController
    {
        Type type = typeof(T);
        if (controllerDict.TryGetValue(type,out BaseController value))
        {
            return value as T;
        }
        
        Debug.LogError($"未找到{type.Name}");
        return null;
    }
}