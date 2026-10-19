using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PeaShooter : Plant
{
    [FormerlySerializedAs("bullet")] public List<Bullet> bullets;

    [SerializeField] protected float mShootTime = 2f;

    [SerializeField] protected float mShootTimer;

    private Coroutine mShootCoroutine;

    protected override void EnableUpdate()
    {
        ShootConstantly();
    }

    private bool HasZombieInRow()
    {
        return ZombieManager.GetInstance().HasAliveZombieInRow(row);
    }

    /// <summary>
    /// 持续射击
    /// </summary>
    protected virtual void ShootConstantly()
    {
        // 索敌条件只影响射击，不中断其他行为的更新。
        if (!CanShoot())
        {
            StopShooting();
            return;
        }

        if (mShootCoroutine != null) return;

        if (mShootTimer >= mShootTime)
        {
            Shoot(0);
            if (bullets.Count > 1)
            {
                mShootCoroutine = StartCoroutine(ShootRemainingBullets());
            }

            mShootTimer = 0;
        }

        mShootTimer += Time.deltaTime;
    }

    protected virtual bool CanShoot()
    {
        return mPlantState == PlantState.Enable && HasZombieInRow();
    }

    protected void Shoot(int i)
    {
        // 延迟连发也要重新索敌，避免当前行清空后继续发射。
        if (!CanShoot()) return;

        Vector3 bulletPosition = transform.position + new Vector3(0.71f, 0.37f, 0);
        Bullet newBullet = Instantiate(bullets[i], bulletPosition, Quaternion.identity,MainGameManager.GetInstance().root.transform);
        newBullet.SetRow(row);
        newBullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0) * newBullet.GetSpeed(), ForceMode2D.Impulse);
    }

    private IEnumerator ShootRemainingBullets()
    {
        for (int i = 1; i < bullets.Count; i++)
        {
            yield return new WaitForSeconds(0.25f);
            if (!CanShoot()) break;
            Shoot(i);
        }

        mShootTimer = 0;
        mShootCoroutine = null;
    }

    private void StopShooting()
    {
        // 只取消连发，保留其他行为启动的协程。
        if (mShootCoroutine != null)
        {
            StopCoroutine(mShootCoroutine);
            mShootCoroutine = null;
        }

        mShootTimer = 0;
    }

    private void OnDisable()
    {
        StopShooting();
    }
}
