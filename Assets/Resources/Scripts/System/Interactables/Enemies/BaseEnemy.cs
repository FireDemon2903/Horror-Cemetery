using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamage, IAlive
{
    public abstract float DMG { get; set; }
    public abstract float Health { get; set; }

    public virtual void TakeDMG(IDamage DMGSource, float? dmg=null)
    {
        if (DMGSource == null) return;

        // if dmg has a value, then use that instead of the normal damage
        Health -= dmg ?? DMGSource.DMG;

        if (Health <= 0) Destroy(gameObject);
    }

    public virtual void DealDMG(IAlive DMGTarget, float? dmg = null)
    {
        DMGTarget.TakeDMG(from: this, dmg: dmg);
    }
}