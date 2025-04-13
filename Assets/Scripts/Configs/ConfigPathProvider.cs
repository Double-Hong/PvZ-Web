using System;
using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    /// <summary>
    /// 配置表路径提供器
    /// </summary>
    public class ConfigPathProvider : IConfigPathProvider
    {
        private static readonly Dictionary<Type, string> ConfigPaths = new Dictionary<Type, string>
        {
            { typeof(PlantInfoConfig), "Config/Plant/PlantInfo" }
        };
    
        public string GetPath(Type type)
        {
            if (ConfigPaths.TryGetValue(type, out string path))
            {
                return path;
            }
    
            Debug.LogError($"{type}未定义配置文件路径,请在ConfigPath中配置");
            return string.Empty;
        }
    }
}