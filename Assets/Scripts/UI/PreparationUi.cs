using System.Collections.Generic;
using GameData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PreparationUi : MonoBehaviour
{
    public GameObject AllCardList;

    public GameObject CardList;

    public GameObject CardTemplate;

    public Button beginBtn;
    
    public TextMeshProUGUI remainSelect;

    public List<GameObject> items;

    private Dictionary<string, List<string>> mPlantInfo;
    
    /// <summary>
    /// 当前选择的卡
    /// </summary>
    private List<GameObject> mCards = new();

    ///TODO:后续改为从玩家配置里面读数量
    /// <summary>
    /// 备选植物数量
    /// </summary>
    [SerializeField] private int mPlantNum = 3;

    private void Awake()
    {
        beginBtn.onClick.AddListener(BeginBtnClick);
        if (MainGameManager.GetInstance().GetCurrentPlayerData() != null)
        {
            mPlantNum = MainGameManager.GetInstance().GetCurrentPlayerData().plantNum;
        }
        ConfigInit();
        RefreshUi();
    }

    void Start()
    {
        
    }

    /// <summary>
    /// 初始化和配置相关的数据
    /// </summary>
    private void ConfigInit()
    {
        //TODO:读取配置的这一部分，都应该封装到他自己的类中
        List<int> playerOwnedPlant = MainGameManager.GetInstance().GetCurrentPlayerData().ownedPlantsId;
        HashSet<int> setPlayerPlant = playerOwnedPlant.ToHashSet();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Atlas/PlantImage/PlantImage");
        Dictionary<string, Sprite> spritesDict = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in sprites)
        {
            spritesDict.Add(sprite.name, sprite);
        }

        for (int i = 0; i < setPlayerPlant.Count; i++)
        {
            GameObject prefab = Instantiate(CardTemplate, AllCardList.transform);
            prefab.SetActive(true);
            PlantInfoConfig config = ConfigManager.GetConfigById<PlantInfoConfig>(playerOwnedPlant[i]);
            prefab.name = config.name;
            Card card = prefab.GetComponent<Card>();
            
            card.InitCard(() =>
            {
                if (mCards.Count >= mPlantNum) return;
                
                GameObject selectedCard = Instantiate(card, CardList.transform).gameObject;
                card.bg.color = Color.black;
                card.elementNode.color = Color.black;
                selectedCard.GetComponent<Card>().InitCard(() =>
                {
                    card.bg.color = Color.white;
                    card.elementNode.color = Color.white;
                    mCards.Remove(prefab);
                    RefreshUi();
                    Destroy(selectedCard);
                }, null,config);
                mCards.Add(prefab);
                RefreshUi();
            }, spritesDict[config.imageName],config);
            
        }
    }

    private void BeginBtnClick()
    {
        Debug.Log("Game Begin!");
        MainGameManager.GetInstance().GameBegin(mCards);
        Destroy(gameObject);
    }

    private void RefreshUi()
    {
        remainSelect.text = $"还可选取<color #FF1515>{mPlantNum - mCards.Count}</color>个植物";
    }
}