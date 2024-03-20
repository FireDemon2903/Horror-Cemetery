using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : Interactor, IEnemy
{
    public float Health { get; set; }
    public float DMG { get; set; }

    internal float detectDisctance = 25f;

    internal bool playerInSight => gameObject.SightTest(PlayerControler.Instance.gameObject, detectDisctance);

    internal delegate void MoveDelegate();

    public virtual void TakeDMG(PlayerControler DMGSource)
    {
        if (DMGSource == null) return;

        if (Health - DMGSource.DMG < 0)
        {
            Destroy(gameObject);
            DMGSource.LastObjectInSight = null;
        }
    }

    public virtual void DealDMG(PlayerControler DMGTarget)
    {
        DMGTarget.Health -= DMG;
    }

}
