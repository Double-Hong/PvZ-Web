using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SunState
{
    /// <summary>
    /// 未被点击
    /// </summary>
    Fall,

    /// <summary>
    /// 被点击
    /// </summary>
    Clicked,

    /// <summary>
    /// 来自向日葵
    /// </summary>
    FromSunflower
}

public class Sun : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// 阳光生成时，阳光最后落下的位置
    /// </summary>
    private Vector3 targetPosition;

    /// <summary>
    /// 阳光的状态
    /// </summary>
    [SerializeField]
    private SunState sunState = SunState.Fall;

    public void SetState(SunState state)
    {
        sunState = state;
    }

    /// <summary>
    /// SunUI的位置
    /// </summary>
    private Vector3 sunUIPosition;

    private AudioSource mAudioSource;

    private void Start()
    {
        sunUIPosition = SunManager.GetInstance().GetSunUIPosition();
        mAudioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        switch (sunState)
        {
            case SunState.Fall:
                Fall();
                break;
            case SunState.Clicked:
                MoveToSunUI();
                break;
            case SunState.FromSunflower:
                
                break;
        }

        // transform.position += (targetPosition-transform.position).normalized * Time.deltaTime * 2;
    }

    /// <summary>
    /// 设置阳光的目标位置
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    
    /// <summary>
    /// 自然下落
    /// </summary>
    private void Fall()
    {
        Vector3 vector3 = Vector3.MoveTowards(transform.position, targetPosition, 2 * Time.deltaTime);
        vector3.Set(vector3.x, vector3.y, (float)-0.1);
        transform.position = vector3;
    }
    
    /// <summary>
    /// 飞向SunUI
    /// </summary>
    private void MoveToSunUI()
    {
        if (Vector3.Distance(transform.position, sunUIPosition) < 0.1f)
        {
            Destroy(gameObject);
            SunManager.GetInstance().GetSun(50);
        }
        else if (Vector3.Distance(transform.position, sunUIPosition) < 1f)
        {
            transform.position += (sunUIPosition - transform.position) * (Time.deltaTime * 12);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, sunUIPosition, 15 * Time.deltaTime);
        }
        
        //TODO:将对象显示在UI之上(已解决)解决方法，将Canvas的摄像机设置为当前摄像机
    }

    /// <summary>
    /// 当阳光被点击时
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        sunState = SunState.Clicked;
        transform.DOKill();
        SunManager.GetInstance().GetSunAudio();
    }

    /// <summary>
    /// 抛物线运动
    /// </summary>
    public void ParabolaMove(Vector3 targetPosition)
    {
        sunState = SunState.FromSunflower;
        Vector3 centerPosition = (transform.position + targetPosition) / 2;
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 1.2f)
        {
            centerPosition.y += 0.6f;
        }
        else
        {
            centerPosition.y += distance/2f;
        }
        transform.DOPath(new[] { transform.position, centerPosition, targetPosition },
            0.8f, PathType.CatmullRom);
    }
}