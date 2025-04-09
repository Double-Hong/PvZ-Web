using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseDialog : MonoBehaviour
{

    public Button ContinueBtn;

    public Button BackMainBtn;

    public Slider MusicSlider;
    
    private void Start()
    {
        ContinueBtn.onClick.AddListener(OnContinueBtnClick);
        BackMainBtn.onClick.AddListener(OnBackMainBtnClick);
        InitMusicSlider();
    }

    public void OnMusicSliderChanged()
    {
        MainGameManager.GetInstance().SetAudioSound(MusicSlider.value);
    }
    
    private void OnContinueBtnClick()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        MainGameManager.GetInstance().SetCamera();
        MainGameManager.GetInstance().RecoverAudio();
        MainGameManager.GetInstance().audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/pause"));
    }

    //TODO 这里和GameOver里面的返回主菜单一样，应该封装到MainGameManager里面去
    private void OnBackMainBtnClick()
    {
        MainGameManager.GetInstance().BackToMain();
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/UserPanel");
        Instantiate(prefab, MainGameManager.GetInstance().GameCanvas.transform);
        Destroy(gameObject);
    }

    private void InitMusicSlider()
    {
        MusicSlider.value = MainGameManager.GetInstance().audioSource.volume;
    }


}
