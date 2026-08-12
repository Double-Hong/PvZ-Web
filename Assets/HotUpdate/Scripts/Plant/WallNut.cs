using UnityEngine;

public class WallNut : Plant
{

    private bool mHurtState;

    private bool mHurtMoreState;

    public override void DamageHealth(int damage)
    {
        base.DamageHealth(damage);
        if (mHealth <= OriginHealth * 0.6)
        {
            mAnimator.SetBool("Hurt",true);
        }

        if (mHealth <= OriginHealth * 0.2)
        {
            mAnimator.SetBool("HurtMore",true);
        }
    }
}
