using UnityEngine;

public enum BulletType
{
    Normal,
    Snow,
}

public class Bullet : MonoBehaviour
{
    /// <summary>
    /// 子弹所在行
    /// </summary>
    [SerializeField] private int row = 0;

    public BulletType bulletType;

    private GameObject mAttackTarget;

    /// <summary>
    /// 获取子弹所在行
    /// </summary>
    /// <returns></returns>
    public int GetRow()
    {
        return row;
    }

    /// <summary>
    /// 设置子弹所在行
    /// </summary>
    /// <param name="row">行数</param>
    public void SetRow(int row)
    {
        this.row = row;
    }

    /// <summary>
    /// 子弹速度
    /// </summary>
    private float speed = 5f;

    public float GetSpeed()
    {
        return speed;
    }

    private int attack = 10;

    private void Update()
    {
        TooFarToDestroy();
    }

    /// <summary>
    /// 当子弹距离中心点的距离大于20时，销毁子弹
    /// </summary>
    private void TooFarToDestroy()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > 20)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Zombie"))
        {
            Zombie zombie = other.GetComponent<Zombie>();
            if (zombie.GetRow() != row || !zombie.IsAlive() || mAttackTarget != null) return;
            mAttackTarget = other.gameObject;
            zombie.PlayAudio("Audio/PeaAttack");
            zombie.DamageHealth(attack,bulletType,null);
            Destroy(gameObject);
        }
    }
}