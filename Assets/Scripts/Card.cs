using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum CardStateEnum
{
    /// <summary>
    /// 冷却中
    /// </summary>
    Cooling,

    /// <summary>
    /// 等待阳光
    /// </summary>
    WaitingSun,

    /// <summary>
    /// 就绪
    /// </summary>
    Ready,

    /// <summary>
    /// 选卡准备阶段
    /// </summary>
    Preparation
}


public class Card : MonoBehaviour
{
    public Image bg;

    public Image elementNode;

    public TextMeshProUGUI consumption;
    
    // public GameObject cardLight;

    // public GameObject cardGrey;

    public Image cardMask;

    public PlantType plantType;

    /// <summary>
    /// 卡片冷却时间
    /// </summary>
    public float cardCooldown = 2.0f;

    /// <summary>
    /// 卡片初始冷却时间
    /// </summary>
    public float startCardCooldown;

    /// <summary>
    /// 被点击时的回调
    /// </summary>
    private Action cardClick;

    /// <summary>
    /// 所需阳光
    /// </summary>
    public int sunNeed = 50;

    /// <summary>
    /// 冷却时间计时器
    /// </summary>
    private float CardCooldownTimer;

    private string mCardId;

    [SerializeField]
    private CardStateEnum CardState = CardStateEnum.Preparation;

    void Start()
    {
        CardCooldownTimer = startCardCooldown;
    }

    void Update()
    {
        switch (CardState)
        {
            case CardStateEnum.Cooling:
                CoolingUpdate();
                break;
            case CardStateEnum.WaitingSun:
                WaitingSunUpdate();
                break;
            case CardStateEnum.Ready:
                ReadyUpdate();
                break;
            case CardStateEnum.Preparation:
                PreparationUpdate();
                break;
        }
    }

    public void InitCard(Action cardClick,Sprite lightCard,PlantInfoConfig config)
    {
        this.cardClick = cardClick;
        cardCooldown = config.cooldown;
        startCardCooldown = config.startCooldown;
        sunNeed = config.sunNeed;
        consumption.text = sunNeed.ToString();
        plantType = Enum.Parse<PlantType>(config.name);
        if (lightCard != null)
        {
            elementNode.sprite = lightCard;
        }
        
    }

    public void PreparationTurnCooling()
    {
        CardState = CardStateEnum.Cooling;
        bg.color = Color.black;
        elementNode.color = Color.black;
        cardMask.gameObject.SetActive(true);
    }

    /// <summary>
    /// 冷却时的Update
    /// </summary>
    private void CoolingUpdate()
    {
        cardMask.fillAmount = CardCooldownTimer / cardCooldown;
        CardCooldownTimer -= Time.deltaTime;
        if (CardCooldownTimer <= 0)
        {
            CoolingTurnWaiting();
        }
    }

    /// <summary>
    /// 等待阳光时的Update
    /// </summary>
    private void WaitingSunUpdate()
    {
        if (SunManager.GetInstance().mSunshineNumber >= sunNeed)
        {
            WaitingTurnReady();
        }
    }

    /// <summary>
    /// 就绪时的Update
    /// </summary>
    private void ReadyUpdate()
    {
        if (SunManager.GetInstance().mSunshineNumber < sunNeed)
        {
            ReadyTurnWaiting();
        }
    }

    /// <summary>
    /// 冷却转换到等待阳光
    /// </summary>
    private void CoolingTurnWaiting()
    {
        Debug.Log("WaitingSun");
        CardState = CardStateEnum.WaitingSun;
        CardCooldownTimer = cardCooldown;
    }

    /// <summary>
    /// 等待阳光转换到就绪
    /// </summary>
    private void WaitingTurnReady()
    {
        CardState = CardStateEnum.Ready;
        bg.color = Color.white;
        elementNode.color = Color.white;
        cardMask.gameObject.SetActive(false);
    }

    /// <summary>
    /// 就绪转换到等待阳光
    /// </summary>
    private void ReadyTurnWaiting()
    {
        CardState = CardStateEnum.WaitingSun;
        bg.color = Color.black;
        elementNode.color = Color.black;
    }

    /// <summary>
    /// 就绪转换到冷却
    /// </summary>
    private void ReadyTurnCooling()
    {
        CardState = CardStateEnum.Cooling;
        bg.color = Color.black;
        elementNode.color = Color.black;
        cardMask.gameObject.SetActive(true);
    }

    /// <summary>
    /// 选卡阶段update
    /// </summary>
    private void PreparationUpdate()
    {
    }

    /// <summary>
    /// 当卡片被点击时
    /// </summary>
    public void OnClick()
    {
        if (CardState == CardStateEnum.Preparation)
        {
            cardClick?.Invoke();
        }
        else
        {
            EffectAudioManager.Instance.PlayEffect("Audio/TakePlant");
            if (SunManager.GetInstance().mSunshineNumber >= sunNeed)
            {
                HandManager.GetInstance().AddPlant(plantType, sunNeed, ReadyTurnCooling);
            }
            //TODO:提示阳光不足
            else
            {
                UIManager.Show("CommonTipsUi","阳光不足");
            }
        }
    }

    private void Test()
    {
        
    }
    
}