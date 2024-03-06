public class GunHandle : Collectable, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(PlayerControler.Parts.GunHandle);
    }
}
