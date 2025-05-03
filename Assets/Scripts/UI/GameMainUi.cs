using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMainUi : MonoBehaviour
{

    public Button pauseBtn;

    public Button gatherSunBtn;

    public Button shovelBtn;

    public GameObject cardList;
    
    public TextMeshProUGUI sunNumber;
    
    private void OnEnable()
    {
        SunManager.ChangeSunNumber += OnSunNumberChanged;
    }

    void Start()
    {
        
        pauseBtn.onClick.AddListener(MainGameManager.GetInstance().OnPauseButtonClick);
        gatherSunBtn.onClick.AddListener(SunManager.GetInstance().GetAllSun);
        shovelBtn.onClick.AddListener(OnShovelBtnClick);
        List<GameObject> cards = MainGameManager.GetInstance().selectedCardList;

        foreach (GameObject card in cards)
        {
            // Instantiate(card, cardList.transform);
            card.transform.SetParent(cardList.transform);
            card.GetComponent<Card>().PreparationTurnCooling();
        }
        shovelBtn.transform.parent.SetSiblingIndex(cards.Count + 1);
    }

    private void OnSunNumberChanged()
    {
        sunNumber.text = SunManager.GetInstance().mSunshineNumber.ToString();
    }

    private void OnShovelBtnClick()
    {
        MainGameManager.GetInstance().ChangeShovelState(true);
        UIManager.Show("ShovelStateUi");
        EffectAudioManager.Instance.PlayEffect("Audio/Shovel");
    }
    
}
