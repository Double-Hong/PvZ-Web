using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager
{
    // 缓存所有配置数据
    private static readonly Dictionary<Type, Dictionary<int, BaseConfig>> configCache = new();

    private static IConfigPathProvider pathProvider;

    public static void SetPathProvider(IConfigPathProvider provider)
    {
        pathProvider = provider;
    }

    private static string GetConfigPath(Type type)
    {
        if (pathProvider == null)
        {
            Debug.LogError("尚未设置配置路径提供者，请调用 ConfigManager.SetPathProvider()");
            return string.Empty;
        }

        return pathProvider.GetPath(type);
    }

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
        string path = GetConfigPath(type);

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