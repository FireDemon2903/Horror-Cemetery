public class GunCyllinder : Collectable, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(PlayerControler.Gunparts.GunCyllinder);
    }
}
