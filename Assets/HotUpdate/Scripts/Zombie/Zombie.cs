using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

enum ZombieState
{
    Move,
    Eat,
    Snow,
    Idle,
}

public class Zombie : MonoBehaviour
{
    /// <summary>
    /// 血量
    /// </summary>
    [SerializeField] private int health = 100;

    /// <summary>
    /// 速度
    /// </summary>
    [SerializeField] private float speed = 0.3f;

    /// <summary>
    /// 设定速度
    /// </summary>
    private float mSpeed;

    /// <summary>
    /// 攻击目标
    /// </summary>
    [SerializeField] private GameObject mAttackTarget;

    /// <summary>
    /// 攻击力
    /// </summary>
    [SerializeField] private int mAttackNumber;
    
    private Animator mAnimator;

    private ZombieState _zombieState = ZombieState.Idle;

    private float mSnowTimer = 0;

    private AudioSource mAudioSource;

    /// <summary>
    /// 僵尸所在行
    /// </summary>
    [SerializeField] private int row = 0;

    /// <summary>
    /// 获取僵尸所在行
    /// </summary>
    /// <returns></returns>
    public int GetRow()
    {
        return row;
    }

    /// <summary>
    /// 设置僵尸所在行
    /// </summary>
    /// <param name="row">行数</param>
    public void SetRow(int row)
    {
        this.row = row;
    }

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        
        mSpeed = speed; 
        StartCoroutine(CheckPosition());
    }

    private void Update()
    {
        switch (_zombieState)
        {
            case ZombieState.Move:
                MoveUpdate();
                break;
            case ZombieState.Eat:
                EatUpdate();
                break;
            case ZombieState.Snow:
                SnowUpdate();
                break;
            case ZombieState.Idle:
                
                break;
        }
    }

    private void SnowUpdate()
    {
        //TODO:做成Buff类，维护性更好
        transform.Translate(Vector3.left * (speed * Time.deltaTime));
        if (mSnowTimer >= 5f)
        {
            TurnToNormal();
        }

        mSnowTimer += Time.deltaTime;
    }

    private void MoveUpdate()
    {
        transform.Translate(Vector3.left * (speed * Time.deltaTime));
    }

    private void EatUpdate()
    {
        if (mAttackTarget == null)
        {
            _zombieState = ZombieState.Move;
            mAttackTarget = null;
            mAnimator.SetBool("Eat", false);
        }
    }

    /// <summary>
    /// 对目标造成伤害
    /// </summary>
    public void HurtTarget()
    {
        if (mAttackTarget != null)
        {
            if (mAttackTarget.CompareTag("Plant"))
            {
                mAttackTarget.GetComponent<Plant>().DamageHealth(mAttackNumber);
                
            }
            else if (mAttackTarget.CompareTag("Zombie"))
            {
                //TODO:后续为魅惑菇一类植物做准备
            }
        }
    }

    public void PlayAttackAudio1()
    {
        AudioClip clip = ZombieManager.GetInstance().GetAudioClip("Audio/ZombieAttack1");
        mAudioSource.PlayOneShot(clip);
    }
    
    public void PlayAttackAudio2()
    {
        AudioClip clip = ZombieManager.GetInstance().GetAudioClip("Audio/ZombieAttack2");
        mAudioSource.PlayOneShot(clip);
    }

    public void DamageHealth(int damage, BulletType? bulletType, [CanBeNull] Action callBack)
    {
        health -= damage;
        if (bulletType == BulletType.Snow)
        {
            TurnToSnow();
        }

        if (health <= 0)
        {
            mAnimator.SetTrigger("LostHead");
            if (callBack != null)
            {
                callBack();
            }
            ZombieManager.GetInstance().ZombieDie(gameObject.transform);
        }
    }

    public void BoomDie()
    {
        speed = 0;
        mAnimator.SetTrigger("Boom");
        ZombieManager.GetInstance().ZombieDie(gameObject.transform);
    }

    public void LostHeadEvent()
    {
        mAnimator.SetTrigger("Die");
        speed = 0;
    }

    public void DieEvent()
    {
        Invoke(nameof(Disappear), (float)1.5);
        GetComponent<Animator>().speed = 0;
    }

    public void Disappear()
    {
        //一秒后销毁
        Destroy(gameObject);
    }

    /// <summary>
    /// 判断僵尸是否存活
    /// </summary>
    /// <returns>存活状态</returns>
    public bool IsAlive()
    {
        //TODO:做成不以血量来判断，做一个僵尸State，用State来判断，因为僵尸血量为0后还有死亡动画可以阻挡子弹
        return health > 0;
    }

    public void PlayAudio(string audioPath)
    {
        AudioClip clip = ZombieManager.GetInstance().GetAudioClip(audioPath);
        mAudioSource.PlayOneShot(clip);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Plant"))
        {
            Plant plant = other.GetComponent<Plant>();
            if (plant.row != row) return;
            mAnimator.SetBool("Eat", true);
            mAttackTarget = other.gameObject;
            _zombieState = ZombieState.Eat;
        }
    }

    //TODO:目前有bug,僵尸在吃植物时，收到寒冰弹，会往前走
    private void TurnToSnow()
    {
        if (_zombieState != ZombieState.Snow)
        {
            speed = mSpeed / 2;
            mAnimator.speed = 0.5f;
            ColorUtility.TryParseHtmlString("#7887FF", out var newColor);
            GetComponent<SpriteRenderer>().color = newColor;
            _zombieState = ZombieState.Snow;
        }

        mSnowTimer = 0;
    }

    private void TurnToNormal()
    {
        //TODO:僵尸动画这一方面，速度方面，耦合性太高了感觉，后面找时间改改
        speed *= 2;
        mAnimator.speed = 1f;
        ColorUtility.TryParseHtmlString("#FFFFFF", out var newColor);
        GetComponent<SpriteRenderer>().color = newColor;
        _zombieState = ZombieState.Move;
    }

    public void TurnToMove()
    {
        _zombieState = ZombieState.Move;
        mAnimator.SetTrigger("Move");
    }



    IEnumerator CheckPosition()
    {
        while (MainGameManager.GetInstance().gameState)
        {
            if (health > 0 && gameObject.transform.localPosition.x <= -11.5)
            {
                MainGameManager.GetInstance().GameFailNormal();
                MainGameManager.GetInstance().gameState = false;
            }
            yield return new WaitForSeconds(1);
        }
    }
}