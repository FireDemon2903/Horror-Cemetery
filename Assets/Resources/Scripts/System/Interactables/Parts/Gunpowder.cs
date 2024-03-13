    public class Gunpowder : Interactor, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(GameManager.Parts.Gunpowder);
    }
}
