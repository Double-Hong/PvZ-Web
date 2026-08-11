namespace Configs
{
    /// <summary>
    /// CDN 资源地址。修改 Root 后需同步更新 MiniGameConfig.asset 中的 CDN 字段。
    /// </summary>
    public static class CdnConfig
    {
        public const string Root = "http://192.168.1.4/abTest/";

        public const string StreamingAssetsPath = "StreamingAssets/";

        public const string StreamingAssetsRoot = Root + StreamingAssetsPath;
#if UNITY_STANDALONE_WIN
        //TODO 临时处理
        public const string ManifestName = "Windows";
#else
        public const string ManifestName = "WebGL";
#endif
    }
}
