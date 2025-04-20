using System.Collections.Generic;
using UnityEngine;

public class UiPathProvider : IUiPathProvider
{
    private Dictionary<string, string> uiPaths = new Dictionary<string, string>
    {
        { "GameOverUi", "Prefabs/UI/GameOverUi/GameOverUi2" },
        {"TestUi","Prefabs/UI/TestUi"}
    };

    public string GetPath(string uiName)
    {
        if (uiPaths.TryGetValue(uiName, out string path))
        {
            return path;
        }

        Debug.LogError($"{uiName}未定义,请在UiPathProvider中配置");
        return string.Empty;
    }
}