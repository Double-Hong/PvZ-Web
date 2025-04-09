using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetWinAwardUi : MonoBehaviour
{

    public TextMeshProUGUI description;

    public Image plantImage;

    public Button nextLevelBtn;

    private void OnEnable()
    {
        nextLevelBtn.onClick.AddListener(NextLevel);
    }

    private void Start()
    {

    }

    public void InitData(int plantNum)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Atlas/PlantImage/PlantImage");
        Dictionary<string, Sprite> spritesDict = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in sprites)
        {
            spritesDict.Add(sprite.name, sprite);
        }
        PlantInfoConfig config = ConfigManager.GetConfigById<PlantInfoConfig>(plantNum);
        string t = "你好你好,{0}";
        description.text = string.Format(t,config.name);
        plantImage.sprite = spritesDict[config.imageName];
    }

    private void NextLevel()
    {
        MainGameManager.GetInstance().OnLevelEnter();
        Destroy(gameObject);
    }

}
