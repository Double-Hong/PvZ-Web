using System.IO;
using Manager.UIManager;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class ABManager
    {
        private static readonly string path = "D:/_Unity Project/PvZ - Web/AssetBundles/WebGL/WebGL";

        [MenuItem("Myh/AbManager/BuildAbJson",false,111)]
        public static void BuildAbJson()
        {
            Debug.Log("BuildAbJson");
            AssetBundle.UnloadAllAssetBundles(true);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            AssetBundleManifest total = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] bundles = total.GetAllAssetBundles();
            AbJson json = new AbJson();
            foreach (string assetName in bundles)
            {
                json.abs.Add(assetName);
            }

            string jsonTxt = JsonUtility.ToJson(json);
            
            File.WriteAllText($"{path}.json",jsonTxt);
            Debug.Log($"已生成AssetBundle资源文件在: {path}.json");
            AssetBundle.UnloadAllAssetBundles(true);
        }
        
        [MenuItem("Myh/AbManager/ReadAbJson",false,112)]
        public static void ReadAbJson()
        {
            Debug.Log("ReadAbJson");
            string json = File.ReadAllText($"{path}.json");
            AbJson abJson = JsonUtility.FromJson<AbJson>(json);
            foreach (var ab in abJson.abs)
            {
                Debug.Log(ab);
            }
        }
    }
}