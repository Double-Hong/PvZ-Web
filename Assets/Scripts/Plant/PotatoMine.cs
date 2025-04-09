using System;
using UnityEngine;

public class PotatoMine : Plant
{
    public GameObject BoomNode;

    public AudioSource audioSource;
    
    /// <summary>
    /// 准备状态
    /// </summary>
    [SerializeField] private bool mReadyState = false;

    /// <summary>
    /// 准备时间
    /// </summary>
    [SerializeField] private float mPrepareTime = 2f;

    /// <summary>
    /// 爆炸范围
    /// </summary>
    private float mBoomRadius = 0.75f;

    /// <summary>
    /// 是否爆炸了
    /// </summary>
    private bool isBoom = false;


    protected override void Update()
    {
        base.Update();
        if (!mReadyState && mPlantState == PlantState.Enable)
        {
            if (mPrepareTime <= 0)
            {
                mReadyState = true;
                mAnimator.SetBool("Ready", true);
            }

            mPrepareTime -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        TriggerDetection(col);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        TriggerDetection(col);
    }

    private void TriggerDetection(Collider2D col)
    {
        if (!mReadyState) return;
        if (col.CompareTag("Zombie"))
        {
            if (isBoom) return;
            
            mAnimator.SetTrigger("Boom");
            BoomNode.SetActive(true);
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/PotatoBoom"));
            
            Collider2D[] collider2Ds =
                Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), mBoomRadius);
            Debug.Log(collider2Ds.Length);
            foreach (Collider2D child in collider2Ds)
            {
                Debug.Log(child.name);
                if (!child.CompareTag("Zombie")) continue;
                
                if (child.GetComponent<Zombie>().GetRow() != row) continue;
                
                child.GetComponent<Zombie>().BoomDie();
            }

            isBoom = true;

            Invoke(nameof(DestroySelf),2f);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}