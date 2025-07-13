using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 配置表导入器
/// </summary>
public class ConfigImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths, bool didDomainReload)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.Contains("/Config/") && assetPath.EndsWith(".txt"))
            {
                Debug.Log(assetPath);
                CreateConfig(assetPath);
            }
        }
    }

    /// <summary>
    /// 创建Config类
    /// </summary>
    /// <param name="path"></param>
    private static void CreateConfig(string path)
    {
        string fileName = Path.GetFileNameWithoutExtension(path); // 类名来源于文件名
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        if (lines.Length < 4)
        {
            Debug.LogError($"配置文件 {fileName} 内容不足，至少需要两行（列名 + 数据）。");
            return;
        }


        // 生成对应的目标路径，替换 Resources/Config -> Scripts/Configs
        string relativePath = path.Replace("Assets/Resources/Config/", "");
        string saveDir = Path.Combine("Assets/Scripts/Configs", Path.GetDirectoryName(relativePath));
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
            Debug.Log($"创建目录: {saveDir}");
        }

        // 提取字段注释（第二行）和字段名（第三行）
        string[] fieldLabels = lines[0].Split('\t');
        string[] fieldComments = lines[1].Split('\t'); // 第二行为字段注释
        string[] fieldTypes = lines[2].Split('\t');
        string[] columnNames = lines[3].Split('\t');   // 第四行为字段名

        string className = fileName; // 类名使用文件名
        string savePath = $"{saveDir}/{className}Config.cs";

        // 生成类文件内容
        StringBuilder sb = new StringBuilder();
        // sb.AppendLine("using Config;");
        sb.AppendLine();
        sb.AppendLine($"public class {className}Config : BaseConfig");
        sb.AppendLine("{");

        for (int i = 0; i < columnNames.Length; i++)
        {
            string fieldLabel = fieldLabels[i].Trim();
            string fieldName = columnNames[i].Trim();
            if (string.IsNullOrEmpty(fieldName)) continue;

            // 获取字段类型
            string fieldType = fieldTypes[i].Trim();

            // 获取字段注释
            string fieldComment = i < fieldComments.Length ? fieldComments[i].Trim() : "字段说明缺失";

            // 添加注释、字段
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {fieldLabel}");
            sb.AppendLine($"    /// {fieldComment}");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    public {fieldType} {fieldName};");
            sb.AppendLine(); // 字段间空一行
        }

        sb.AppendLine("}");

        // 保存生成的类文件
        File.WriteAllText(savePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"生成配置类: {savePath}");
        AssetDatabase.Refresh();
    }
    
    
}