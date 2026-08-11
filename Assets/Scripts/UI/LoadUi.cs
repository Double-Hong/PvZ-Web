using Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadUi : MonoBehaviour
{

    public Slider slider;

    public async UniTask<bool> BeginLoadAb()
    {
        var ab = AssetBundle.LoadFromFile(CdnConfig.StreamingAssetsRoot + CdnConfig.ManifestName);
        AssetBundleManifest total = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] bundles = total.GetAllAssetBundles();
        slider.maxValue = bundles.Length + 1;
        await LoadAb(bundles);
        return true;
    }

    private async UniTask LoadAb(string[] bundles)
    {
        UnityWebRequest request = null;
        foreach (var bundle in bundles)
        { 
            request = UnityWebRequestAssetBundle.GetAssetBundle($"{CdnConfig.StreamingAssetsRoot}{bundle}");
            var res = await request.SendWebRequest();
            if (res.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("/ERROR/" + request.error);
                Debug.LogError($"{bundle}下载失败");
            }
        }
        
        request?.Dispose();
    }
}
