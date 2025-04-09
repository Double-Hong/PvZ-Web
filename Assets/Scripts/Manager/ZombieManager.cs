using System;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

public class ZombieManager : MonoBehaviour
{
    private LevelData _levelData;

    private ZombieWave currentWave;

    private List<Transform> positions;

    private GameObject line;

    private GameObject[] lines = new GameObject[5];

    private const float ZOMBIE_TIME = 5;

    private Transform lastDieTransform;

    /// <summary>
    /// 僵尸生成间隔
    /// </summary>
    [SerializeField] private float zombieTime = ZOMBIE_TIME;

    /// <summary>
    /// 僵尸生成计时器
    /// </summary>
    [SerializeField] private float zombieTimer;

    [SerializeField]
    private bool lastZombieFlag = false;

    [SerializeField]
    private bool checkLastFlag = true;

    [SerializeField]
    private int zombieNumber = 0;

    private Dictionary<string, AudioClip> mZombieAudioManager;

    private static ZombieManager INSTANCE;

    private ZombieManager()
    {
        INSTANCE = this;
    }

    public static ZombieManager GetInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new ZombieManager();
        }

        return INSTANCE;
    }

    private void OnEnable()
    {
        zombieNumber = 0;
        mZombieAudioManager = new Dictionary<string, AudioClip>();
    }

    private void Update()
    {
        ProduceZombieUpdate();
        if (lastZombieFlag && checkLastFlag)
        {
            CheckLastZombieDie();
        }
    }


    public void DestroyLine()
    {
        Destroy(line);
    }

    public void InitAll()
    {
        Init();
        InitLineInfo();
        ReadLevelInfo();
    }
    
    private void Init()
    {
        zombieTime = ZOMBIE_TIME;
        zombieTimer = 0;
        lastZombieFlag = false;
        checkLastFlag = true;
    }

    /// <summary>
    /// 初始化僵尸行
    /// </summary>
    private void InitLineInfo()
    {
        line = new GameObject();
        line.transform.SetParent(MainGameManager.GetInstance().root.transform);
        line.name = "Lines";
        GameObject clone = Resources.Load<GameObject>("Prefabs/Zombie/ZombieLine");
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = Instantiate(clone, line.transform);
            lines[i].name = "ZombieLine" + (i + 1);
            SortingGroup sortingGroup = lines[i].GetComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Game";
            sortingGroup.sortingOrder = lines.Length - i;
        }
    }

    /// <summary>
    /// 读取关卡信息
    /// </summary>
    private void ReadLevelInfo()
    {
        _levelData = Resources.Load<LevelData>($"GameData/MainLevel/Level{MainGameManager.GetInstance().GetCurrentPlayerData().MainLevel.ToString()}");
        positions = _levelData.positions;
        _levelData.index = 0;
        if (_levelData.waves.Count > 0)
        {
            currentWave = _levelData.waves[_levelData.index];
            zombieTime = currentWave.spawnTime;
            zombieTimer = zombieTime;
        }
    }

    private void ProduceZombieUpdate()
    {
        if (_levelData.index <= _levelData.waves.Count)
        {
            if (zombieTimer > 0)
            {
                if (_levelData.index < _levelData.waves.Count)
                {
                    zombieTimer -= Time.deltaTime;
                }
            }
            else
            {
                // for (int i = 0; i < currentWave.zombies.Count; i++)
                // {
                    ProduceZombie();
                // }
                if (_levelData.index < _levelData.waves.Count)
                {
                    currentWave = _levelData.waves[_levelData.index];
                    zombieTime = currentWave.spawnTime;
                    zombieTimer = zombieTime;
                }
                else
                {
                    lastZombieFlag = true;
                    zombieTimer = 9999;
                }
            }
        }
    }

    private void ProduceZombie()
    {
        Random random = new Random();
        int row = random.Next(0, 5);
        Zombie zombie = Resources.Load<Zombie>("Prefabs/Zombie/Zombie");
        zombie.SetRow(row);
        zombie = Instantiate(zombie, positions[row].localPosition, new Quaternion(), lines[row].transform);
        zombieNumber++;
        zombie.TurnToMove();
        _levelData.index++;
    }

    private void CheckLastZombieDie()
    {
        if (zombieNumber == 0)
        {
            MainGameManager.GetInstance().GameWin(Camera.main.WorldToScreenPoint(lastDieTransform.position),_levelData);
            checkLastFlag = false;
        }
    }

    public void ZombieDie(Transform trans)
    {
        zombieNumber--;
        lastDieTransform = trans;
    }

    public AudioClip GetAudioClip(string path)
    {
        if (!mZombieAudioManager.ContainsKey(path))
        {
            AudioClip clip = Resources.Load<AudioClip>(path);
            mZombieAudioManager.Add(path, clip);
            return clip;
        }

        return mZombieAudioManager[path];
    }
}