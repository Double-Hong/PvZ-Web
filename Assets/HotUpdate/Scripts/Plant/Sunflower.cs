using UnityEngine;

public class Sunflower : Plant
{
    public float produceSunTime = 8.0f;

    public float sunTimer = 0;

    /// <summary>
    /// 该向日葵产生的阳光数量(用于排序)
    /// </summary>
    private int mSunNumber = 10;

    protected override void EnableUpdate()
    {
        ProduceSunTime();
    }
    
    private void ProduceSunTime()
    {
        if (sunTimer >= produceSunTime)
        {
            GetComponent<Animator>().SetTrigger("IsProduceSun");
            sunTimer = 0;
        }

        sunTimer += Time.deltaTime;
    }
    
    public void ProduceSun()
    {
        GameObject sun = Instantiate(Resources.Load<GameObject>("Prefabs/SomeObject/Sun"), transform.position,
            Quaternion.identity,SunManager.GetInstance().sunList.transform);
        sun.GetComponent<Sun>().SetTargetPosition(transform.position);
        sun.GetComponent<SpriteRenderer>().sortingOrder = mSunNumber++;
        float random = Random.Range(-1f, 1f);
        sun.GetComponent<Sun>().ParabolaMove(transform.position+new Vector3(random,0,0));
    }
}
