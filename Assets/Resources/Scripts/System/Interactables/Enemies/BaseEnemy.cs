public class BaseEnemy : Interactor, IEnemy
{
    public float Health { get; set; } = 1;
    public float DMG { get; set; } = 10;

    public void TakeDMG(PlayerControler DMGSource)
    {
        if (DMGSource == null) return;

        if (Health - DMGSource.DMG < 0)
        {
            Destroy(gameObject);
            DMGSource.LastObjectInSight = null;
        }
    }

    public void DealDMG(PlayerControler DMGTarget)
    {
        DMGTarget.Health -= DMG;
    }
}
