public class GunHandle : Interactor, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.GunHandle);
    }
}
