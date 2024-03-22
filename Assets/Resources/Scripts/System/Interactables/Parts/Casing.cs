public class Casing : Interactor, IPart
{
    public void Collect(PlayerController playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.Casing);
    }
}
