using System.Collections.Generic;
using UnityEngine;

public class PlantModel:BaseModel
{

    private static PlantModel inst;
    public static PlantModel Inst
    {
        get
        {
            if (inst == null)
            {
                inst = new PlantModel();
            }

            return inst;
        }
    }
    
    
    /// <summary>
    /// 植物图集
    /// </summary>
    private Dictionary<string, Sprite> plantSprites = new();

    public void Init()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Atlas/PlantImage/PlantImage");
        foreach (Sprite sprite in sprites)
        {
            plantSprites.Add(sprite.name, sprite);
        }
    }

    /// <summary>
    /// 获取图
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Sprite GetSpriteByName(string name)
    {
        if (plantSprites.TryGetValue(name,out Sprite value))
        {
            return value;
        }
        
        Debug.LogError($"图集中未找到{name}");
        return null;
    }
    
    
}