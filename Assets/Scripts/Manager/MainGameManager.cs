using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    public Canvas GameCanvas;

    public Camera MyCamera;

    public AudioSource audioSource;

    public SunManager sunManager;

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

    private void Awake()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/UserPanel");
        Instantiate(prefab, GameCanvas.transform);
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
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            PreparationMoveCamera((() => Debug.Log("k")));
        }

        timer += Time.deltaTime;
        
    }

    private void OnGUI()
    {
        // if (GUI.Button(new Rect(10, 10, 50, 50), "btnTexture"))
        //     Debug.Log("Clicked the button with an image");
        //
        // EditorGUI.LabelField(new Rect(300, 300, 100, 20), "Label Text");
        // bool toggleTxt = false;
        // toggleTxt = GUILayout.Toggle(toggleTxt, "A Toggle text");
    }

    public void SetCamera()
    {
        GameCanvas.worldCamera = MyCamera;
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

        GameCanvas.GetComponent<Canvas>().worldCamera = null;
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
        SetCamera();
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
        foreach (GameObject card in cards)
        {
            Debug.Log(card.name);
        }
    }

    private void SetZombieManagerState(bool state)
    {
        GetComponent<ZombieManager>().enabled = state;
    }

    private void SetSunManagerState(bool state)
    {
        sunManager.enabled = state;
    }

    private void InitGameBeginEvents()
    {
        GameBeginEvent += () => GameCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        GameBeginEvent += () =>
        {
            GameObject mainGameUi = Resources.Load<GameObject>("Prefabs/UI/GameMainUi");
            mainGameUi = Instantiate(mainGameUi, GameCanvas.transform);
            MainGameUi = mainGameUi;
        };
        GameBeginEvent += () => sunManager.InitAll();
        GameBeginEvent += () => ZombieManager.GetInstance().InitAll();
        GameBeginEvent += () => SetZombieManagerState(true);
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
        GameBackToMainEvent += ZombieManager.GetInstance().DestroyLine;
        GameBackToMainEvent += () => sunManager.OnBackToMenuEvent();
        GameBackToMainEvent += () => gameState = false;
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
        GameCanvas.GetComponent<Canvas>().worldCamera = null;
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
                Sprite[] sprites = Resources.LoadAll<Sprite>("Atlas/PlantImage/PlantImage");
                Dictionary<string, Sprite> spritesDict = new Dictionary<string, Sprite>();
                foreach (Sprite sprite in sprites)
                {
                    spritesDict.Add(sprite.name, sprite);
                }

                int index = int.Parse(levelData.winAward.plantId);
                PlantInfoConfig config = ConfigManager.GetConfigById<PlantInfoConfig>(index);
                GameObject cardTemplate = Resources.Load<GameObject>("Prefabs/UI/CardTemplate");
                GameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                GameObject prefab = Instantiate(cardTemplate,lastZombieTransform,Quaternion.identity, MainGameUi.transform);
                GameCanvas.renderMode = RenderMode.ScreenSpaceCamera;

                prefab.SetActive(true);
                // prefab.transform.SetPositionAndRotation(new Vector3(prefab.transform.position.x,prefab.transform.position.y,0),Quaternion.identity);
                Card card = prefab.GetComponent<Card>();
                card.InitCard(() =>
                {
                    Debug.Log("显示获取界面");
                    Time.timeScale = 0;
                    ShowGainAwardUi(index);
                },spritesDict[config.imageName],config);
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
        GameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
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
    
    
    
}