public class GunBarrel : Interactor, IPart
{
    public void Collect(PlayerController playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.GunBarrel);
    }
}
