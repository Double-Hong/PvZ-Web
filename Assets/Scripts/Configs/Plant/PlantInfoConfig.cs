using Config;

public class PlantInfoConfig : BaseConfig
{
    /// <summary>
    /// 植物id
    /// 植物唯一id
    /// </summary>
    public int id;

    /// <summary>
    /// 名称
    /// 植物在Unity显示的名称，与对应枚举相同
    /// </summary>
    public string name;

    /// <summary>
    /// 图的名字
    /// 对应图的名称
    /// </summary>
    public string imageName;

    /// <summary>
    /// 灰色图的名字
    /// 灰色图的名字
    /// </summary>
    public string greyName;

    /// <summary>
    /// 冷却时间
    /// 植物的一般冷却时间
    /// </summary>
    public int cooldown;

    /// <summary>
    /// 开始冷却时间
    /// 开局时植物的冷却时间
    /// </summary>
    public int startCooldown;

    /// <summary>
    /// 所需阳光
    /// 所需阳光
    /// </summary>
    public int sunNeed;

}
