using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update

    private void OnEnable()
    {
        ChangeUserBtn.onClick.AddListener(OpenLogin);
        SureBtn.onClick.AddListener(CloseLogin);
        BeginGameBtn.onClick.AddListener(GameBeginBtnClick);
        QuitBtn.onClick.AddListener(QuitBtnClick);
        GameOverBtn.onClick.AddListener(GameOverBtnClick);
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

        MainGameManager.GetInstance().OnLevelEnter();

        Destroy(gameObject);
    }

    private void GameOverBtnClick()
    {
        MainGameManager.GetInstance().GameFailNormal();
        Destroy(gameObject);
    }

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
