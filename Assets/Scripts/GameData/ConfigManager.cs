using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Config;
using Configs;
using UnityEngine;

namespace GameData
{
    public class ConfigManager
    {
        private static readonly string environmentPath = Environment.CurrentDirectory;

        /// <summary>
        /// 读取配置文件(使用的StreamReader)
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns>带有属性名和值列表的字典</returns>
        public static Dictionary<string, List<string>> LoadConfig(string path)
        {
            string fullPath = environmentPath + path;
            
            // fullPath = Path.Combine(Application.streamingAssetsPath, "Config/Plant/AllPlantInfo.txt");
            Debug.LogError(fullPath);
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string[] keys = { };

            try
            {
                // 读取所有行
                string[] lines = File.ReadAllLines(fullPath);
                if (lines.Length < 3)
                {
                    throw new InvalidOperationException("配置文件格式不正确，行数不足。");
                }

                // 提取键名（第3行）
                keys = lines[2].Split("\t");
                foreach (string key in keys)
                {
                    result[key] = new List<string>();
                }

                // 从第4行开始解析数据
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    string[] dataLine = lines[i].Split("\t");
                    if (dataLine.Length != keys.Length)
                    {
                        throw new InvalidOperationException($"数据行与键数量不匹配。行：{i + 1}");
                    }

                    for (int j = 0; j < keys.Length; j++)
                    {
                        result[keys[j]].Add(dataLine[j]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置文件时出错：{e.Message}");
                throw;
            }

            return result;
        }
        
        /// <summary>
        /// 读取配置文件(使用的StreamReader)
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns>带有属性名和值列表的字典</returns>
        public static Dictionary<string, List<string>> LoadConfigResources(string path)
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Config/Plant/AllPlantInfo");
            // fullPath = Path.Combine(Application.streamingAssetsPath, "Config/Plant/AllPlantInfo.txt");
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string[] keys = { };

            try
            {
                // 读取所有行
                string[] lines = textAsset.text.Split("\r\n");
                if (lines.Length < 3)
                {
                    throw new InvalidOperationException("配置文件格式不正确，行数不足。");
                }

                // 提取键名（第3行）
                keys = lines[2].Split("\t");
                foreach (string key in keys)
                {
                    result[key] = new List<string>();
                }

                // 从第4行开始解析数据
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    string[] dataLine = lines[i].Split("\t");
                    if (dataLine.Length != keys.Length)
                    {
                        throw new InvalidOperationException($"数据行与键数量不匹配。行：{i + 1}");
                    }

                    for (int j = 0; j < keys.Length; j++)
                    {
                        result[keys[j]].Add(dataLine[j]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置文件时出错：{e.Message}");
                throw;
            }

            return result;
        }
        

        /// <summary>
        /// 将Dictionary&lt;string, List&lt;string&gt;&gt;中List的第index个的数据解析成Dictionary&lt;string, string&gt;
        /// </summary>
        /// <param name="config">原数据</param>
        /// <param name="index">第几个</param>
        /// <returns>Dictionary&lt;string, string&gt;</returns>
        public static Dictionary<string, string> AnalysisConfig(Dictionary<string, List<string>> config, int index)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var kvp in config)
            {
                if (index < 0 || index >= kvp.Value.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "索引超出范围。");
                }

                result[kvp.Key] = kvp.Value[index];
            }

            return result;
        }
        
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="config"></param>
        public static void SaveConfig(string path, Dictionary<string, List<string>> config)
        {
            string fullPath = environmentPath + path;

            using StreamWriter sw = new StreamWriter(fullPath);
            // 写入标题行
            sw.WriteLine(string.Join("\t", config.Keys));

            // 写入数据行
            int rowCount = config.Values.First().Count;
            for (int i = 0; i < rowCount; i++)
            {
                var row = config.Select(kvp => kvp.Value[i]).ToArray();
                sw.WriteLine(string.Join("\t", row));
            }
        }
        
        // 缓存所有配置数据
    private static readonly Dictionary<Type, Dictionary<int, BaseConfig>> configCache = new();

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="id">配置id</param>
    /// <typeparam name="T">配置对应的类</typeparam>
    /// <returns></returns>
    public static T GetConfigById<T>(int id) where T : BaseConfig, new()
    {
        Type type = typeof(T);

        // 如果缓存中没有该类型的配置，先加载
        if (!configCache.ContainsKey(type))
        {
            LoadConfig<T>();
        }
        
        // 从缓存中查找指定 id 的配置
        if (configCache[type].TryGetValue(id, out BaseConfig config))
        {
            return config as T;
        }

        Debug.LogError($"未找到 {type.Name} 中 id 为 {id} 的配置");
        return null;
    }

    /// <summary>
    /// 加载配置数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static void LoadConfig<T>() where T : BaseConfig, new()
    {
        Type type = typeof(T);
        string configName = type.Name.Replace("Config", "");
        string path = ConfigPath.GetConfigPath(type);

        if (path == string.Empty) return;

        TextAsset configFile = Resources.Load<TextAsset>(path);
        if (configFile == null)
        {
            Debug.LogError($"无法加载配置文件: {path}");
            return;
        }

        string[] lines = configFile.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        if (lines.Length < 4)
        {
            Debug.LogError($"配置文件 {configName} 内容不足，至少需要四行数据。");
            return;
        }

        // 解析字段名（第三行）
        string[] columnNames = lines[3].Split('\t');

        // 创建缓存
        var configDict = new Dictionary<int, BaseConfig>();

        // 从第五行开始读取数据
        for (int i = 4; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split('\t');
            T config = new T();

            for (int j = 0; j < columnNames.Length && j < values.Length; j++)
            {
                var field = type.GetField(columnNames[j]);
                if (field == null) continue;

                object value = ConvertValue(field.FieldType, values[j]);
                field.SetValue(config, value);
            }

            // 获取 id 作为 key
            int id = (int)type.GetField("id").GetValue(config);
            configDict[id] = config;
        }

        // 缓存该类型的配置
        configCache[type] = configDict;
        Debug.Log($"已加载 {type.Name} 配置，共 {configDict.Count} 条记录");
    }

    // 根据字段类型转换值
    private static object ConvertValue(Type targetType, string value)
    {
        //TODO 数组类型还未对应
        if (targetType == typeof(int)) return int.Parse(value);
        if (targetType == typeof(float)) return float.Parse(value);
        if (targetType == typeof(bool)) return bool.Parse(value);
        return value; // 默认为 string 类型
    }

    }
}