public class Casing : Interactor, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.Casing);
    }
}
