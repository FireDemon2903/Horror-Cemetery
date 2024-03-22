public class GunCyllinder : Interactor, IPart
{
    public void Collect(PlayerController playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.GunCyllinder);
    }
}
