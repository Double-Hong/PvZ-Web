using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMainUi : MonoBehaviour
{

    public Button pauseBtn;

    public Button gatherSunBtn;

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
        List<GameObject> cards = MainGameManager.GetInstance().selectedCardList;

        foreach (GameObject card in cards)
        {
            // Instantiate(card, cardList.transform);
            card.transform.SetParent(cardList.transform);
            card.GetComponent<Card>().PreparationTurnCooling();
        }
    }

    private void OnSunNumberChanged()
    {
        sunNumber.text = SunManager.GetInstance().mSunshineNumber.ToString();
    }
    
}
