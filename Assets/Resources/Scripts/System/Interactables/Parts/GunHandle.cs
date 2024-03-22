public class GunHandle : Interactor, IPart
{
    public void Collect(PlayerController playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.GunHandle);
    }
}
