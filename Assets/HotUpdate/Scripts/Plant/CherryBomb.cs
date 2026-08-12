
using UnityEngine;

public class CherryBomb : Plant
{

    public GameObject bomb;

    private float mBoomRadius = 2f;

    public void Bomb()
    {
        mAnimator.SetTrigger("Bomb");
        EffectAudioManager.Instance.PlayEffect("Audio/CherryBomb");
        GetComponent<SpriteRenderer>().enabled = false;
        bomb.SetActive(true);
        Destroy(gameObject,1.5f);
        Collider2D[] collider2Ds =
            Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), mBoomRadius);
        foreach (Collider2D child in collider2Ds)
        {
            Debug.Log(child.name);
            if (!child.CompareTag("Zombie")) continue;
            
            child.GetComponent<Zombie>().BoomDie();
        }
    }
}
