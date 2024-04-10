using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamage, IAlive
{
    public abstract float DMG { get; set; }
    public abstract float Health { get; set; }

    public virtual void TakeDMG(IDamage DMGSource)
    {
        if (DMGSource == null) return;

        if (Health - DMGSource.DMG <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void DealDMG(IAlive DMGTarget)
    {
        DMGTarget.TakeDMG(from: this);
    }
}