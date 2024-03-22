    public class Gunpowder : Interactor, IPart
{
    public void Collect(PlayerController playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.Gunpowder);
    }
}
