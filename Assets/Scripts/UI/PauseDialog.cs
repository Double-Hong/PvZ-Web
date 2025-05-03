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

    public Slider EffectSlider;
    
    private void Start()
    {
        ContinueBtn.onClick.AddListener(OnContinueBtnClick);
        BackMainBtn.onClick.AddListener(OnBackMainBtnClick);
        InitMusicSlider();
    }

    private void OnContinueBtnClick()
    {
        SaveMusicAndEffect();
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        MainGameManager.GetInstance().SetCameraLow();
        MainGameManager.GetInstance().RecoverAudio();
        MainGameManager.GetInstance().audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/pause"));
    }

    //TODO 这里和GameOver里面的返回主菜单一样，应该封装到MainGameManager里面去
    private void OnBackMainBtnClick()
    {
        SaveMusicAndEffect();
        EffectAudioManager.Instance.PlayEffect("Audio/ButtonClick");
        MainGameManager.GetInstance().BackToMain();
        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/UserPanel");
        Instantiate(prefab, MainGameManager.GetInstance().GameCanvas.transform);
        Destroy(gameObject);
    }

    /// <summary>
    /// 保存音乐和音效大小
    /// </summary>
    private void SaveMusicAndEffect()
    {
        MainGameManager.GetInstance().SetAudioSound(MusicSlider.value);
        EffectAudioManager.Instance.SetVolume(EffectSlider.value);
    }

    private void InitMusicSlider()
    {
        MusicSlider.value = MainGameManager.GetInstance().audioSource.volume;
        EffectSlider.value = EffectAudioManager.Instance.GetVolume();
    }


}
