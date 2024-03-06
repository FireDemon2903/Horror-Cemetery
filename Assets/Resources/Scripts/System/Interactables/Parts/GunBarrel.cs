public class GunBarrel : Collectable, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(PlayerControler.Parts.GunBarrel);
    }
}
