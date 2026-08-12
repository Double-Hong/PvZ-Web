using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.Networking;

public class GameEntry : MonoBehaviour
{
    public static GameEntry Instance { get; private set; }

    [Header("是否启用AB")]
    public bool isAb;
    
    public Canvas gameCanvas;
    public GameObject root;
    public RectTransform rootRect;
    public CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
        // Editor下无需加载，直接查找获得HotUpdate程序集
        
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        Type type = hotUpdateAss.GetType("Hello");
        type.GetMethod("Run").Invoke(null, null);
    }
}