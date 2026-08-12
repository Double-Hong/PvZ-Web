using UnityEngine;

public interface IUiPathProvider
{
    /// <summary>
    /// 获取预制件路径
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    string GetPath(string uiName);
}