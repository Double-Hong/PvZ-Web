using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Level Data", order = 0)]
    public class LevelData : ScriptableObject
    {
        /// <summary>
        /// 每一波的僵尸数据
        /// </summary>
        [Header("波次")]
        public List<ZombieWave> waves;
        
        [Header("索引")]
        public int index = 0;

        [Header("僵尸生成位置")]
        public List<Transform> positions;
        
        [Header("奖励类型")]
        public WinAwardType winAwardType;

        [Header("奖励内容")]
        public WinAward winAward;

    }

    [Serializable]
    public class ZombieWave
    {
        /// <summary>
        /// 这一波僵尸的生成时间
        /// </summary>
        public float spawnTime;

        /// <summary>
        /// 僵尸的类型和数量
        /// </summary>
        public List<ZombieInfo> zombies;
    }

    [Serializable]
    public class ZombieInfo
    {
        /// <summary>
        /// 僵尸类型（可以是一个枚举）
        /// </summary>
        public ZombieType zombieType;

        /// <summary>
        /// 这一波中生成的僵尸数量
        /// </summary>
        public int count;
    }

    [Serializable]
    public class WinAward
    {
        [Header("植物id")]
        public string plantId;
        [Header("消息")]
        public string message;
        [Header("金币")]
        public int gold;
    }

    public enum ZombieType
    {
        BasicZombie,
        ConeheadZombie,
        BucketheadZombie,
        // 其他僵尸类型
    }

    [Flags]
    public enum WinAwardType
    {
        None = 0,
        Plant = 1 << 0,
        Slot = 1 << 1,
        Message = 1 << 2 ,
        Gold = 1 << 3,
    }
    
}