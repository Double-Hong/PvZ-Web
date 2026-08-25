using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameData;
using myh;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif
using UnityEngine.UI;

public class MainGameManager : MonoSingleton<MainGameManager>
{
    [Header("是否启用AB")]
    public bool isAssetBundle;

    public LoadUi loadUi;
    
    public Canvas GameCanvas;

    public Camera MyCamera;

    public AudioSource audioSource;
    
    public GameObject root;

    public GameObject MainGameUi;

    public RectTransform rootRect;

    public CanvasScaler cs;

    public List<GameObject> selectedCardList;

    public CinemachineVirtualCamera virtualCamera;

    public Material CardMaterial;

    public bool isFinishMainLevel => mCurrentPlayerData.MainLevel > mMainLevelNum;

    private int mMainLevelNum;

    private GameObject mPauseDialog;

    private PlayerData mCurrentPlayerData;

    private float mAudioStartTime;

    private AudioClip mCurrentClip;

    private bool mGameState = true;

    /// <summary>
    /// 是否拿起铁铲
    /// </summary>
    private bool mShovelState = false;

    public bool gameState
    {
        get => mGameState;
        set => mGameState = value;
    }

    public PlayerData GetCurrentPlayerData()
    {
        return mCurrentPlayerData;
    }

    private static MainGameManager Instance;

    private MainGameManager()
    {
        Instance = this;
    }

    public static MainGameManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new MainGameManager();
        }

        return Instance;
    }

    protected override void ResetData()
    {
        Debug.Log("ResetData");
    }

    protected override void Init()
    {
        Debug.Log("Init MainGameManager");
        Debug.Log($"GameEntry.isAb = {GameEntry.Instance.isAb}");
        isAssetBundle = GameEntry.Instance.isAb;
        GameCanvas = GameEntry.Instance.gameCanvas;
        root = GameEntry.Instance.root;
        rootRect = GameEntry.Instance.rootRect;
        virtualCamera = GameEntry.Instance.virtualCamera;
        audioSource = GetInstance().gameObject.AddComponent<AudioSource>();
        SendToLoadAb().Forget();
    }

    protected override void Destroy()
    {
        Debug.Log("OnDestroy");
    }

    private async UniTask SendToLoadAb()
    {
        try
        {
            await UIManager.Init(new UiPathProvider(), rootRect);
            // loadUi.gameObject.SetActive(false);
            ConfigManager.SetPathProvider(new ConfigPathProvider());
            EffectAudioManager.InitSingleton();
            // GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/UserPanel");
            UIManager.ShowTest<UserPanel>("UserPanel");
            // Instantiate(prefab, GameCanvas.transform);
        }
        catch (Exception e)
        {
            Debug.LogError($"游戏启动加载失败: {e.Message}");
        }
    }

    private void Start()
    {
        //TODO:Init的位置可能要改
        InitGameBeginEvents();
        InitGameQuitEvents();
        mMainLevelNum = GetMainLevelNum();
        Application.runInBackground = true;
    }

    private float timer = 0;

#if UNITY_EDITOR
    /// <summary>
    /// 【Profiler 练习】仅编辑器生效，默认关闭。打开后每帧会故意跑一段很慢的代码，用来学习 Unity Profiler。
    /// 开启方式：Inspector 勾选，或运行时按 P。练完务必关掉，否则游戏会明显卡顿。
    /// </summary>
    [Header("Profiler 练习（默认关闭，练完请保持关闭）")]
    [SerializeField]
    private bool enableProfilerDemo = false;
#endif
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            PreparationMoveCamera((() => Debug.Log("k")));
        }

#if UNITY_EDITOR
        // 按 P 开关 Profiler 练习。Hierarchy 路径：
        // PlayerLoop → Update.ScriptRunBehaviourUpdate → MainGameManager.Update() → ProfilerDemo/HeavyWork
        if (Input.GetKeyDown(KeyCode.P))
        {
            enableProfilerDemo = !enableProfilerDemo;
            Debug.Log($"Profiler 练习已{(enableProfilerDemo ? "开启" : "关闭")}");
        }

        if (enableProfilerDemo)
        {
            ProfilerDemo_HeavyWork();
        }
#endif

        timer += Time.deltaTime;
        
    }

#if UNITY_EDITOR
    #region Profiler 练习（学习用，仅编辑器编译，正式包不会带上）

    /// <summary>
    /// 总入口。Profiler CPU Hierarchy 里会看到这条，以及下面三个子采样。
    /// Total 时间 ≈ 三个子函数之和；Self 时间接近 0，因为自己几乎不干活。
    /// </summary>
    private void ProfilerDemo_HeavyWork()
    {
        Profiler.BeginSample("ProfilerDemo/HeavyWork");
        ProfilerDemo_BurnCpu();
        ProfilerDemo_AllocateGarbage();
        ProfilerDemo_NestedMath();
        Profiler.EndSample();
    }

    /// <summary>
    /// 纯 CPU 计算：每帧 30 万次 Sqrt/Sin。
    /// 预期：Time ms 很高（大约十几毫秒），GC Alloc = 0。
    /// 对应真实项目：过深的循环、复杂数学、同步计算。
    /// </summary>
    private void ProfilerDemo_BurnCpu()
    {
        Profiler.BeginSample("ProfilerDemo/BurnCpu");
        float sum = 0f;
        for (int i = 0; i < 300000; i++)
        {
            sum += Mathf.Sqrt(i) * Mathf.Sin(i);
        }

        // 用一下结果，防止编译器把循环优化掉
        if (sum < 0f)
        {
            Debug.Log(sum);
        }

        Profiler.EndSample();
    }

    /// <summary>
    /// 每帧大量分配托管对象：new List + 字符串拼接。
    /// 预期：Time ms 很低（不到 1ms），但 GC Alloc 大约几十 KB。
    /// 过一会儿会触发 GC.Collect，那一帧会突然卡一下。
    /// 对应真实项目：每帧 new、字符串拼接、装箱、GetComponent 缓存缺失。
    /// </summary>
    private void ProfilerDemo_AllocateGarbage()
    {
        Profiler.BeginSample("ProfilerDemo/AllocateGarbage");
        var list = new List<string>(1000);
        for (int i = 0; i < 800; i++)
        {
            list.Add("plant_" + i + "_" + Time.frameCount);
        }

        Profiler.EndSample();
    }

    /// <summary>
    /// 再套一层调用栈，用来观察 Profiler 父子层级。
    /// 预期：出现在 HeavyWork 下面，Time ms 大约几毫秒，GC Alloc = 0。
    /// </summary>
    private void ProfilerDemo_NestedMath()
    {
        Profiler.BeginSample("ProfilerDemo/NestedMath");
        float acc = 0f;
        for (int i = 0; i < 100000; i++)
        {
            acc += Mathf.Pow(i % 17, 2) / (i + 1);
        }

        if (acc < 0f)
        {
            Debug.Log(acc);
        }

        Profiler.EndSample();
    }

    #endregion
#endif

    private void OnGUI()
    {
        // if (GUI.Button(new Rect(10, 10, 50, 50), "btnTexture"))
        //     Debug.Log("Clicked the button with an image");
        //
        // EditorGUI.LabelField(new Rect(300, 300, 100, 20), "Label Text");
        // bool toggleTxt = false;
        // toggleTxt = GUILayout.Toggle(toggleTxt, "A Toggle text");
    }

    public void SetCameraLow()
    {
        GameCanvas.sortingOrder = 1;
        GameCanvas.sortingLayerName = "Game";
    }

    public void SetCameraHigh()
    {
        GameCanvas.sortingOrder = 999;
        GameCanvas.sortingLayerName = "Foreground";
    }

    public void OnPauseButtonClick()
    {
        Time.timeScale = 0f;
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/pause"));
        mAudioStartTime = audioSource.time;
        audioSource.clip = null;
        ShowPauseDialog();
    }

    public void RecoverAudio()
    {
        audioSource.clip = mCurrentClip;
        audioSource.time = mAudioStartTime;
        audioSource.Play();
    }

    public void SetAudioSound(float value)
    {
        audioSource.volume = value;
    }

    private void ShowPauseDialog()
    {
        if (mPauseDialog == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/PauseDialog");
            mPauseDialog = Instantiate(prefab, new Vector3(0, 0, GameCanvas.transform.position.z), new Quaternion(),
                GameCanvas.transform);
        }
        else
        {
            mPauseDialog.SetActive(true);
        }

        SetCameraHigh();
    }

    private int GetMainLevelNum()
    {
        LevelData[] zombieWaveData = Resources.LoadAll<LevelData>("GameData/MainLevel");
        return zombieWaveData.Length;
    }
    
    /// <summary>
    /// 刚进入关卡时的初始化处理
    /// </summary>
    public void OnLevelEnter()
    {
        ClearRoot();
        Destroy(MainGameUi);
        SetZombieManagerState(false);
        SetSunManagerState(false);
        Time.timeScale = 1;
        SetCameraLow();
        GameObject background1 = Resources.Load<GameObject>("Prefabs/SomeObject/Background1");
        Instantiate(background1, root.transform);
        PreparationMoveCamera(() =>
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/PreparationUi");
            Instantiate(prefab, GameCanvas.transform);
        });
        SetMainMusic("Audio/PreparationMusic");
    }

    #region 游戏开始时触发的事件

    /// <summary>
    /// 游戏开始时触发的事件
    /// </summary>
    private static Action GameBeginEvent;

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="cards">准备界面选择的植物卡</param>
    public void GameBegin(List<GameObject> cards)
    {
        GameBeginEvent?.Invoke();
        selectedCardList = cards;
        // foreach (GameObject card in cards)
        // {
        //     Debug.Log(card.name);
        // }
    }

    private void SetZombieManagerState(bool state)
    {
        ZombieManager.GetInstance().SetAvailable(state);
    }

    private void SetSunManagerState(bool state)
    {
        SunManager.GetInstance().enabled = state;
    }

    private void InitGameBeginEvents()
    {
        GameBeginEvent += () => GameCanvas.sortingOrder = 1;
        GameBeginEvent += () =>
        {
            GameObject mainGameUi = Resources.Load<GameObject>("Prefabs/UI/GameMainUi");
            mainGameUi = Instantiate(mainGameUi, GameCanvas.transform);
            MainGameUi = mainGameUi;
        };
        GameBeginEvent += () => SunManager.GetInstance().InitAll();
        GameBeginEvent += () => ZombieManager.GetInstance().SetAvailable(true);
        // GameBeginEvent += () => ;
        GameBeginEvent += () => SetSunManagerState(true);
        GameBeginEvent += () => SetMainMusic("Audio/bgm1");
        GameBeginEvent += () =>
        {
            virtualCamera.transform.DOMove(new Vector3(0, 0, -10), 1.5f);
            Debug.Log("左移1");
        };
        GameBeginEvent += () =>
        {
            Time.timeScale = 1f;
            gameState = true;
        };
        //TODO:UI方面的也要初始化
    }

    private void SetMainMusic(string path)
    {
        mCurrentClip = Resources.Load<AudioClip>(path);
        audioSource.clip = mCurrentClip;
        audioSource.time = 0;
        audioSource.Play();
    }

    #endregion

    #region 游戏中返回主菜单发生的事件

    /// <summary>
    /// 游戏中返回主菜单发生的事件
    /// </summary>
    private Action GameBackToMainEvent;

    public void BackToMain()
    {
        GameBackToMainEvent?.Invoke();
    }

    private void InitGameQuitEvents()
    {
        GameBackToMainEvent += () => Destroy(MainGameUi);
        GameBackToMainEvent += () => SetZombieManagerState(false);
        GameBackToMainEvent += () => SetSunManagerState(false);
        GameBackToMainEvent += ClearRoot;
        GameBackToMainEvent += ()=> ZombieManager.GetInstance().DestroyLine();
        GameBackToMainEvent += () => SunManager.GetInstance().OnBackToMenuEvent();
        GameBackToMainEvent += () => gameState = false;
        GameBackToMainEvent += () => Time.timeScale = 1f;
    }

    #endregion


    /// <summary>
    /// 加载玩家数据
    /// </summary>
    /// <param name="playerData"></param>
    public void LoadPlayerData(PlayerData playerData)
    {
        mCurrentPlayerData = playerData;
    }

    /// <summary>
    /// 普通的游戏结束
    /// </summary>
    public void GameFailNormal()
    {
        Debug.Log("游戏失败 ! ! !");
        Time.timeScale = 0;
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/GameOverUi/GameOverUi");
        Instantiate(prefab, GameCanvas.transform);
        audioSource.clip = null;
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/losemusic"));
        SetCameraHigh();
    }

    /// <summary>
    /// 准备阶段选卡时，移动摄像机
    /// </summary>
    public void PreparationMoveCamera(Action callback)
    {
        StartCoroutine(MoveCamera(callback));
    }

    IEnumerator MoveCamera(Action callback)
    {
        virtualCamera.transform.position = new Vector3(0, 0, -10);
        virtualCamera.transform.DOMove(new Vector3(-2, 0, -10), 1.5f, false);
        Debug.Log("左移");
        yield return new WaitForSeconds(3f);
        virtualCamera.transform.DOMove(new Vector3(2.5f, 0, -10), 2.5f, false);
        Debug.Log("右移");
        yield return new WaitForSeconds(2.5f);
        callback();
        yield return null;
    }

    /// <summary>
    /// 清楚Root中所有对象
    /// </summary>
    public void ClearRoot()
    {
        for (int i = 0; i < root.transform.childCount; i++)
        {
            Destroy(root.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    public void GameWin(Vector3 lastZombieTransform,LevelData levelData = null)
    {
        if (levelData != null)
        {
            if ((levelData.winAwardType & WinAwardType.Plant) == WinAwardType.Plant)
            {


                int index = int.Parse(levelData.winAward.plantId);
                PlantInfoConfig config = ConfigManager.GetConfigById<PlantInfoConfig>(index);
                
                GameObject cardTemplate = Resources.Load<GameObject>("Prefabs/UI/CardTemplate");
                GameObject prefab = Instantiate(cardTemplate,MainGameUi.transform);
                Vector2 vector2 = new Vector2(lastZombieTransform.x, lastZombieTransform.y);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, vector2, Camera.main,
                    out Vector2 vector);
                prefab.GetComponent<RectTransform>().anchoredPosition = vector;
                
                prefab.SetActive(true);
                // prefab.transform.SetPositionAndRotation(new Vector3(prefab.transform.position.x,prefab.transform.position.y,0),Quaternion.identity);
                Card card = prefab.GetComponent<Card>();
                card.InitCard(() =>
                {
                    Debug.Log("显示获取界面");
                    Time.timeScale = 0;
                    ShowGainAwardUi(index);
                },PlantModel.Inst.GetSpriteByName(config.imageName),config);
                Debug.Log("Plant");
                mCurrentPlayerData.ownedPlantsId.Add(int.Parse(levelData.winAward.plantId));
            }

            if ((levelData.winAwardType & WinAwardType.Slot) == WinAwardType.Slot)
            {
                Debug.Log("Slot");
                mCurrentPlayerData.plantNum++;
            }

            if ((levelData.winAwardType & WinAwardType.Message) == WinAwardType.Message)
            {
                Debug.Log("Message");
                Debug.Log(levelData.winAward.message);
            }

            if ((levelData.winAwardType & WinAwardType.Gold) == WinAwardType.Gold)
            {
                Debug.Log("Gold");
                mCurrentPlayerData.playerGold += levelData.winAward.gold;
            }
            
        }

        mCurrentPlayerData.MainLevel++;
        PlayerData.WritePlayerData(mCurrentPlayerData);
        Debug.Log("Win!");
        SetCameraHigh();
    }

    private void ShowGainAwardUi(int plantId)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/GetWinAwardUi");
        prefab = Instantiate(prefab, GameCanvas.transform);
        GetWinAwardUi ui = prefab.GetComponent<GetWinAwardUi>();
        ui.InitData(plantId);
        // if (!isFinishMainLevel)
        // {
        //     OnLevelEnter();
        // }
    }

    #region 铁铲

    /// <summary>
    /// 是否拿起铁铲
    /// </summary>
    /// <returns></returns>
    public bool IsTakeShovel()
    {
        return mShovelState;
    }

    /// <summary>
    /// 改变铁铲状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeShovelState(bool state)
    {
        mShovelState = state;
        if (state)
        {
            SetCameraHigh();
        }
        else
        {
            SetCameraLow();
        }
        
    }

    #endregion
    
    
    
}