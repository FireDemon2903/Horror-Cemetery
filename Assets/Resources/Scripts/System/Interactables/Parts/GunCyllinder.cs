public class GunCyllinder : Interactor, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.GunCyllinder);
    }
}
