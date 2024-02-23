public class Gunpowder : Collectable, IPart
{
    public void Collect(PlayerControler playerControler)
    {
        playerControler.OwnedParts.Add(PlayerControler.Bulletparts.Gunpowder);
    }
}
