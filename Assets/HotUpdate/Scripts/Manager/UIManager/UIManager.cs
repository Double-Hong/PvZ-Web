using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configs;
using Cysharp.Threading.Tasks;
using Manager.UIManager;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public class UIManager
{
    private static IUiPathProvider uiPathProvider;

    private static Transform root;

    private static Dictionary<string, BaseView> viewDict = new();

    private static Dictionary<string, AssetBundle> assetBundles = new();

    private static bool isAb;
    
    /// <summary>
    /// 初始化UI路径提供器
    /// </summary>
    /// <param name="provider">路径提供器</param>
    /// <param name="rootTransform">UI根路径</param>
    public static async UniTask Init(IUiPathProvider provider,Transform rootTransform)
    {
        SetUiPathProvider(provider);
        root = rootTransform;
        isAb = MainGameManager.GetInstance().isAssetBundle;
        if (!isAb)
        {
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        await BeginLoadAbJson();
#else
        await BeginLoadAbLocal();
#endif
    }
    
    public static async UniTask<bool> BeginLoadAb()
    {
        AssetBundle ab = null;
        Debug.Log(CdnConfig.StreamingAssetsRoot + CdnConfig.ManifestName);
        //这里是加载ab主包，从主包里面获取其他ab包信息
        using UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(CdnConfig.StreamingAssetsRoot + CdnConfig.ManifestName + ".json");
        // 等待请求结束（无需协程）
        var res = await req.SendWebRequest();

        if (res.result == UnityWebRequest.Result.Success)
        {
            ab = (res.downloadHandler as DownloadHandlerAssetBundle)?.assetBundle;
            AssetBundleManifest total = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] bundles = total.GetAllAssetBundles();
            await LoadAbRemote(bundles.ToList());
            return true;
        }
        else
        {
            Debug.LogError("下载失败: " + res.error);
            return false;
        }
    }

    public static async UniTask<bool> BeginLoadAbJson()
    {
        var url = CdnConfig.Root + CdnConfig.ManifestName + ".json";
        Debug.Log(url);
        try
        {
            using UnityWebRequest req = UnityWebRequest.Get(url);
            req.timeout = 30;
            var res = await req.SendWebRequest();

            if (res.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(res.downloadHandler.text);
                var abJson = JsonUtility.FromJson<AbJson>(res.downloadHandler.text);
                await LoadAbRemote(abJson.abs);
                return true;
            }

            Debug.LogError("下载失败: " + res.error);
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError("下载失败: " + e.Message);
            return false;
        }
    }

    private static async UniTask<bool> BeginLoadAbLocal()
    {
        string manifestPath = Path.Combine(Application.streamingAssetsPath, CdnConfig.ManifestName);
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestPath);
        if (manifestBundle == null)
        {
            Debug.LogError($"本地 Manifest 加载失败: {manifestPath}");
            return false;
        }

        AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var bundles = manifest.GetAllAssetBundles().ToList();
        manifestBundle.Unload(false);
        await LoadAbLocal(bundles);
        return true;
    }
    
    private static async UniTask<uint> GetVersionAsync()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(CdnConfig.Root + "version.txt"))
        {
            // 等待请求结束（无需协程）
            await req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"version = {req.downloadHandler.text}");
                return uint.Parse(req.downloadHandler.text);
            }
            else
            {
                Debug.LogError("下载失败: " + req.error);
                return 0;
            }
        }
    }

    private static async UniTask LoadAbRemote(List<string> bundles)
    {
        UnityWebRequest request = null;
        foreach (var bundle in bundles)
        {
            float startTime = Time.time;
            request = UnityWebRequestAssetBundle.GetAssetBundle($"{CdnConfig.StreamingAssetsRoot}{bundle}");
            var res = await request.SendWebRequest();
            float cost = (Time.time - startTime) * 1000f;
            if (res.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("/ERROR/" + request.error);
                Debug.LogError($"{bundle}下载失败");
            }
            else
            {
                Debug.Log($"包名:{bundle}");
                Debug.Log($"请求成功,耗时:{cost} ms");
                AssetBundle ab = (res.downloadHandler as DownloadHandlerAssetBundle)?.assetBundle;
                assetBundles.Add(bundle.Split('_')[0], ab);
            }
        }

        request?.Dispose();
        Debug.Log("延迟1s");
        await UniTask.Delay(1000);
    }

    private static async UniTask LoadAbLocal(List<string> bundles)
    {
        foreach (var bundle in bundles)
        {
            float startTime = Time.time;
            string path = Path.Combine(Application.streamingAssetsPath, bundle);
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            float cost = (Time.time - startTime) * 1000f;
            if (ab == null)
            {
                Debug.LogError($"{bundle} 本地加载失败: {path}");
            }
            else
            {
                Debug.Log($"包名:{bundle}");
                Debug.Log($"加载成功,耗时:{cost} ms");
                assetBundles.Add(bundle.Split('_')[0], ab);
            }

            await UniTask.Yield();
        }

        Debug.Log("延迟1s");
        await UniTask.Delay(1000);
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

        if (isAb)
        {
            Debug.Log("ab加载");
            if (assetBundles.TryGetValue("ui",out var ab))
            {
                GameObject ui = ab.LoadAsset<GameObject>(uiName);
                ui = (GameObject)Object.Instantiate(ui, root);
                BaseView bv = ui.AddComponent<CommonTipsUi>();
                viewDict.Add(uiName,bv);
                bv.Show(args);
            }
            else
            {
                ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/ui");
                GameObject ui = ab.LoadAsset<GameObject>(uiName);
                ui = (GameObject)Object.Instantiate(ui, root);
                BaseView bv = ui.AddComponent<CommonTipsUi>();
                viewDict.Add(uiName,bv);
                assetBundles.Add("ui",ab);
                bv.Show(args);
            }
        }
        else
        {
            GameObject ui = Resources.Load<GameObject>(path);
        
            ui = (GameObject)Object.Instantiate(ui, root);
            BaseView bv = ui.GetComponent<BaseView>();
            viewDict.Add(uiName,bv);
            bv.Show(args);
        }
        

    }

    public static void Show<T>(string uiName,params object[] args) where T : BaseView
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

        if (isAb)
        {
            Debug.Log("ab加载");
            LoadAb<T>(uiName, args).Forget();
        }
        else
        {
            if (viewDict.TryGetValue(uiName,out var v))
            {
                var bv = v.GetComponent<T>();
                bv.Show(args);
            }
            else
            {
                GameObject ui = Resources.Load<GameObject>(path);
        
                ui = (GameObject)Object.Instantiate(ui, root);
                var bv = ui.AddComponent<T>();
                viewDict.Add(uiName,bv);
                bv.Show(args);
            }
        }

    }
    
    public static void ShowTest<T>(string uiName,params object[] args) where T : BaseView
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

        if (isAb)
        {
            Debug.Log("ab加载");
            LoadAbTest<T>(uiName, args).Forget();
        }
        else
        {
            if (viewDict.TryGetValue(uiName,out var v))
            {
                var bv = v.GetComponent<T>();
                bv.Show(args);
            }
            else
            {
                GameObject ui = Resources.Load<GameObject>(path);
        
                ui = (GameObject)Object.Instantiate(ui, root);
                var bv = ui.AddComponent<T>();
                viewDict.Add(uiName,bv);
                bv.Show(args);
            }
        }

    }
    
    private static async UniTask LoadAbTest<T>(string uiName,params object[] args) where T : BaseView
    {
        Debug.Log("ab加载");
        if (assetBundles.TryGetValue(uiName.ToLower(),out var ab))
        {
            GameObject ui = ab.LoadAsset<GameObject>(uiName);
            ui = (GameObject)Object.Instantiate(ui, root);
            T bv = ui.GetComponent<T>();
            bv.Show(args);
            
            viewDict.Add(uiName,bv);
            // T bv = ui.AddComponent<T>();
            // bv.Show(args);
        }
        else
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle($"{CdnConfig.StreamingAssetsRoot}ui");
            var res = await request.SendWebRequest();
            if (res.isHttpError)
            {
                Debug.LogError("/ERROR/" + request.error);
            }
            else
            {
                AssetBundle abs = (res.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                GameObject ui = abs.LoadAsset<GameObject>(uiName);
                ui = (GameObject)Object.Instantiate(ui, root);
                T bv = ui.AddComponent<T>();
                viewDict.Add(uiName,bv);
                assetBundles.Add("ui",abs);
                bv.Show(args);
                // abs.Unload(false);
            }
            request.Dispose();
        }
    }

    private static async UniTask LoadAb<T>(string uiName,params object[] args) where T : BaseView
    {
        Debug.Log("ab加载");
        if (assetBundles.TryGetValue("ui",out var ab))
        {
            GameObject ui = ab.LoadAsset<GameObject>(uiName);
            ui = (GameObject)Object.Instantiate(ui, root);
            T bv = ui.AddComponent<T>();
            viewDict.Add(uiName,bv);
            bv.Show(args);
        }
        else
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle($"{CdnConfig.StreamingAssetsRoot}ui");
            var res = await request.SendWebRequest();
            if (res.isHttpError)
            {
                Debug.LogError("/ERROR/" + request.error);
            }
            else
            {
                AssetBundle abs = (res.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                GameObject ui = abs.LoadAsset<GameObject>(uiName);
                ui = (GameObject)Object.Instantiate(ui, root);
                T bv = ui.AddComponent<T>();
                viewDict.Add(uiName,bv);
                assetBundles.Add("ui",abs);
                bv.Show(args);
                // abs.Unload(false);
            }
            request.Dispose();
        }
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