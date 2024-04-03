public class Casing : Interactor, IPart
{
    public void Collect(PlayerController playerController)
    {
        playerController.OwnedParts.Add(GameManager.Parts.Casing);
    }
}
