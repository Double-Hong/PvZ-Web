using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SunManager : MonoBehaviour
{
    public int mSunshineNumber;

    /// <summary>
    /// 阳光值改变事件
    /// </summary>
    public delegate void SunValueHandler();

    /// <summary>
    /// 阳光值改变事件
    /// </summary>
    public static event SunValueHandler ChangeSunNumber;

    public GameObject sunList;

    public Transform root;

    private static SunManager Instance;

    private SunManager()
    {
        Instance = this;
    }

    public static SunManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SunManager();
        }

        return Instance;
    }

    /// <summary>
    /// 天空落下阳光的间隔时间
    /// </summary>
    [SerializeField] private float sunProduceTime = 5f;

    /// <summary>
    /// 天空生产阳光的计时器
    /// </summary>
    private float produceSunTimer = 0f;

    /// <summary>
    /// SunList是否存在
    /// </summary>
    private bool isSunList => sunList != null;

    /// <summary>
    /// 左边界
    /// </summary>
    private float leftPoint = -7f;

    /// <summary>
    /// 右边界
    /// </summary>
    private float rightPoint = 4f;

    /// <summary>
    /// 上边界
    /// </summary>
    private float topPoint = 2f;

    /// <summary>
    /// 下边界
    /// </summary>
    private float bottomPoint = -4f;
    
    private IEnumerator sunCoroutine;

    /// <summary>
    /// 阳光UI的位置
    /// </summary>
    [SerializeField] private Vector3 sunNumberPosition = new(-7.5f, 4.2f, 0);

    public Vector3 GetSunUIPosition()
    {
        return sunNumberPosition;
    }

    private void OnEnable()
    {
        // InitAll();
    }

    public void InitAll()
    {
        if (sunCoroutine != null)
        {
            StopCoroutine(sunCoroutine);
        }
        mSunshineNumber = 50;
        onSunValueChanged();
        sunList = new GameObject();
        sunList.name = "SunList";
        sunList.transform.SetParent(root);
        sunCoroutine = ProduceSun(5);
        StartCoroutine(sunCoroutine);
    }
    
    private void Update()
    {
        // ProduceSunUpdate();
    }

    /// <summary>
    /// 
    /// </summary>
    private void onSunValueChanged()
    {
        ChangeSunNumber?.Invoke();
    }

    /// <summary>
    /// 获得阳光
    /// </summary>
    /// <param name="sunNumber">获得阳光数量</param>
    public void GetSun(int sunNumber)
    {
        mSunshineNumber += sunNumber;
        onSunValueChanged();
    }

    /// <summary>
    /// 播放获取阳光时的音效
    /// </summary>
    public void GetSunAudio()
    {
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Audio/GetSun"));
    }

    /// <summary>
    /// 使用阳光
    /// </summary>
    /// <param name="sunNumber">使用阳光数量</param>
    public void UseSun(int sunNumber)
    {
        mSunshineNumber -= sunNumber;
        onSunValueChanged();
    }

    /// <summary>
    /// 生产阳光的Update
    /// </summary>
    private void ProduceSunUpdate()
    {
        if (produceSunTimer >= sunProduceTime)
        {
            ProduceSunFromSky();
            produceSunTimer = 0;
            // sunProduceTime = UnityEngine.Random.Range(6, 10);
            sunProduceTime = Random.Range(5, 10);
        }

        produceSunTimer += Time.deltaTime;
    }

    
    
    /// <summary>
    /// 使用协程生产阳光
    /// </summary>
    /// <param name="time">生产间隔</param>
    /// <returns></returns>
    IEnumerator ProduceSun(int time)
    {
        while (true)
        {
            sunProduceTime = time;
            time = Random.Range(5, 10);
            yield return new WaitForSeconds(time);
            ProduceSunFromSky();
        }
    }

    public void OnBackToMenuEvent()
    {
        Destroy(sunList);
        if (sunCoroutine != null)
        {
            StopCoroutine(sunCoroutine);
        }
    }

    /// <summary>
    /// 生产来自天空的阳光
    /// </summary>
    private void ProduceSunFromSky()
    {
        if (!isSunList) return;
        
        float x = Random.Range(leftPoint, rightPoint);
        float y = Random.Range(bottomPoint, topPoint);
        Vector3 targetPosition = new Vector3(x, y, 0);
        GameObject sun = Instantiate(Resources.Load<GameObject>("Prefabs/SomeObject/Sun"), new Vector3(x, 5, 0),
            Quaternion.identity, sunList.transform);
        sun.GetComponent<Sun>().SetTargetPosition(targetPosition);
    }

    /// <summary>
    /// 停止天空生产阳光
    /// </summary>
    public void StopSunFromSky()
    {
        sunProduceTime = 9999;
    }

    /// <summary>
    /// 收集场上所有阳光
    /// </summary>
    public void GetAllSun()
    {
        Sun[] suns = sunList.GetComponentsInChildren<Sun>();
        foreach (Sun sun in suns)
        {
            sun.SetState(SunState.Clicked);
        }

        GetSunAudio();
    }
}