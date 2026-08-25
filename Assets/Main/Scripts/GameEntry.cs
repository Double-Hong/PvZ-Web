using System;
using System.Linq;
using System.Reflection;
using Cinemachine;
using Cysharp.Threading.Tasks;
using HybridCLR;
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

    /// <summary>
    /// 与 HybridCLRGenerate/AOTGenericReferences.PatchedAOTAssemblyList 保持同步。
    /// 对应 dll 需放到 CDN StreamingAssets（文件名形如 mscorlib.dll.bytes）。
    /// </summary>
    static readonly string[] AotDlls =
    {
        "mscorlib.dll",
        "System.Core.dll",
        "UniTask.dll",
        "UnityEngine.AssetBundleModule.dll",
        "UnityEngine.CoreModule.dll",
        "UnityEngine.JSONSerializeModule.dll",
        "myh.commontools.dll",
        "myh.configmanager.dll",
    };

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if UNITY_EDITOR
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
        RunHello(hotUpdateAss);
#else
        // WebGL / 微信小游戏的 streamingAssetsPath 是 HTTP URL，必须用 UnityWebRequest，不能用 File IO
        BootAsync().Forget();
#endif
    }

#if !UNITY_EDITOR
    async UniTaskVoid BootAsync()
    {
        await LoadMetadataForAotAssemblies();

        string url = $"{Application.streamingAssetsPath}/HotUpdate.dll.bytes";
        using UnityWebRequest req = UnityWebRequest.Get(url);
        var res = await req.SendWebRequest();
        if (res.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"加载 HotUpdate.dll 失败: {req.error}\nurl={url}");
            return;
        }

        Assembly hotUpdateAss = Assembly.Load(res.downloadHandler.data);
        RunHello(hotUpdateAss);
    }

    async UniTask LoadMetadataForAotAssemblies()
    {
        foreach (string aotDll in AotDlls)
        {
            string url = $"{Application.streamingAssetsPath}/{aotDll}.bytes";
            using UnityWebRequest req = UnityWebRequest.Get(url);
            var res = await req.SendWebRequest();
            if (res.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"补充元数据失败，无法下载: {aotDll}\n{req.error}\nurl={url}");
                continue;
            }

            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(res.downloadHandler.data, HomologousImageMode.SuperSet);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDll} ret:{err}");
        }
    }
#endif

    static void RunHello(Assembly hotUpdateAss)
    {
        Type type = hotUpdateAss.GetType("Hello");
        type.GetMethod("Run").Invoke(null, null);
    }
}
