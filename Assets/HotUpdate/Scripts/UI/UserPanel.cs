using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using WeChatWASM;

public class UserPanel : MonoBehaviour
{

    public TextMeshProUGUI PlayerName;

    public Button ChangeUserBtn;

    public GameObject LoginPanel;

    public TMP_InputField NameInput;

    public Button SureBtn;

    public Button BeginGameBtn;

    public Button QuitBtn;

    public Button GameOverBtn;

    public TextMeshProUGUI GoldNumber;

    public bool player;
    
    private string filePath;

    private PlayerData playerData;

    public Button testBtn;

    // Start is called before the first frame update

    private void OnEnable()
    {
        ChangeUserBtn.onClick.AddListener(OpenLogin);
        SureBtn.onClick.AddListener(CloseLogin);
        BeginGameBtn.onClick.AddListener(GameBeginBtnClick);
        QuitBtn.onClick.AddListener(QuitBtnClick);
        GameOverBtn.onClick.AddListener(GameOverBtnClick);
        testBtn.onClick.AddListener(TestBtnClick);
    }

    void Start()
    {
        // FitScreen();
        if (player)
        {
            PlayerPrefs.DeleteAll();
        }
        else
        {
            string playerName = PlayerPrefs.GetString("LastPlayerName");
            if (playerName == "")
            {
                LoginPanel.SetActive(true);
            }
            else
            {
                LoadPlayerData(playerName);
            }
        }

        
        // ABTest();
    }

    private void ABTest()
    {
        var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/WebGL");
        AssetBundleManifest abmf = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        abmf.GetAllAssetBundles();
    }

    private void OpenLogin()
    {
        NameInput.text = PlayerName.text;
        LoginPanel.SetActive(true);
    }

    private void LoadPlayerData(string playerName)
    {
        PlayerName.text = playerName;
        PlayerData data = PlayerData.GetPlayerData(playerName);

        // https://github.com/wechat-miniprogram/minigame-unity-webgl-transform/tree/main/Demo
        playerData = data;
        GoldNumber.text = data.playerGold.ToString();
        MainGameManager.GetInstance().LoadPlayerData(data);
    }
    
    private void CloseLogin()
    {
        if (player)
        {
            
        }
        else
        {
            PlayerName.text = NameInput.text;
            PlayerPrefs.SetString("LastPlayerName",NameInput.text);
            //TODO:重新读取数据
            LoadPlayerData(NameInput.text);
        }
        LoginPanel.SetActive(false);
    }

    private void GameBeginBtnClick()
    {
        if (MainGameManager.GetInstance().isFinishMainLevel)
        {
            Debug.Log("已完成全部主线关卡");
            return;
        }
        EffectAudioManager.Instance.PlayEffect("Audio/ButtonClick");
        MainGameManager.GetInstance().OnLevelEnter();

        Destroy(gameObject);
    }

    private void GameOverBtnClick()
    {
        MainGameManager.GetInstance().GameFailNormal();
        // EffectAudioManager.Instance.PlayEffect("Audio/ButtonClick");
        // UIManager.Show("TestUi");
        Destroy(gameObject);
        Debug.Log(Application.dataPath);
        AssetBundle ab = AssetBundle.LoadFromFile(Application.dataPath + "/StreamingAssets/prefab/plant");
        Debug.Log(ab.name);
        GameObject one = ab.LoadAsset<GameObject>("PeaShooter");
        Instantiate(one);
    }
    
    private void TestBtnClick()
    {
        UIManager.ShowTest<TestUi>("TestUi");
    }

    /// <summary>
    /// 调用云函数示例
    /// </summary>
    // private void Test()
    // {
    //     WXBase.cloud.CallFunction(new CallFunctionParam()
    //     {
    //         name = "add",
    //         data = new
    //         {
    //             a = 1,
    //             b = 2
    //         },
    //         success = (res) =>
    //         {
    //             Debug.Log("调用成功");
    //         },
    //         fail = (error) =>
    //         {
    //             Debug.LogError("调用失败");
    //         }
    //     });
    // }

    private void QuitBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        Application.Quit();
#elif minigame
        // WX.ExitMiniProgram(null);
#endif
    }
}
