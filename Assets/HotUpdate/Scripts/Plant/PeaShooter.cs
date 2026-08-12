using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PeaShooter : Plant
{
    [FormerlySerializedAs("bullet")] public List<Bullet> bullets;

    [SerializeField] protected float mShootTime = 2f;

    [SerializeField] protected float mShootTimer;

    protected override void EnableUpdate()
    {
        ShootConstantly();
    }

    /// <summary>
    /// 持续射击
    /// </summary>
    protected virtual void ShootConstantly()
    {
        if (mShootTimer >= mShootTime)
        {
            Shoot(0);
            for (int i = 1; i < bullets.Count; i++)
            {
                StartCoroutine(ShootDelay(i));
            }

            mShootTimer = 0;
        }

        mShootTimer += Time.deltaTime;
    }

    protected void Shoot(int i)
    {
        Vector3 bulletPosition = transform.position + new Vector3(0.71f, 0.37f, 0);
        Bullet newBullet = Instantiate(bullets[i], bulletPosition, Quaternion.identity,MainGameManager.GetInstance().root.transform);
        newBullet.SetRow(row);
        newBullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0) * newBullet.GetSpeed(), ForceMode2D.Impulse);
    }

    IEnumerator ShootDelay(int i)
    {
        yield return new WaitForSeconds(i * 0.25f);
        Shoot(i);
        if (i == bullets.Count - 1)
        {
            mShootTimer = 0;
        }
    }
}