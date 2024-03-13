public class GunBarrel : Interactor, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.GunBarrel);
    }
}
